using LibraryManagement.API.Data;
using LibraryManagement.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace LibraryManagement.API.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationUser> AuthenticateAsync(string email, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

            if (user == null || !VerifyPasswordHash(password, user.PasswordHash))
                return null;

            return user;
        }

        public async Task<ApplicationUser> RegisterAsync(ApplicationUser user, string password)
        {
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                throw new Exception("Email is already taken");

            user.PasswordHash = CreatePasswordHash(password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<IEnumerable<BorrowedBook>> GetBorrowedBooksAsync(string userId)
        {
            return await _context.BorrowedBooks
                .Where(bb => bb.UserId == userId && bb.ReturnDate == null)
                .Include(bb => bb.Book)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetReservationsAsync(string userId)
        {
            return await _context.Reservations
                .Where(r => r.UserId == userId)
                .Include(r => r.Book)
                .ToListAsync();
        }

        public async Task<BorrowedBook> BorrowBookAsync(string userId, int bookId, int days)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null || !book.IsAvailable)
                throw new Exception("Book is not available for borrowing");

            var borrowedBook = new BorrowedBook
            {
                UserId = userId,
                BookId = bookId,
                BorrowDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(days)
            };

            book.IsAvailable = false;
            _context.BorrowedBooks.Add(borrowedBook);
            await _context.SaveChangesAsync();

            return borrowedBook;
        }

        public async Task ReturnBookAsync(string userId, int bookId)
        {
            var borrowedBook = await _context.BorrowedBooks
                .SingleOrDefaultAsync(bb => bb.UserId == userId && bb.BookId == bookId && bb.ReturnDate == null);

            if (borrowedBook == null)
                throw new Exception("Book not found or already returned");

            borrowedBook.ReturnDate = DateTime.UtcNow;
            var book = await _context.Books.FindAsync(bookId);
            book.IsAvailable = true;

            await _context.SaveChangesAsync();
        }

        public async Task<Reservation> ReserveBookAsync(string userId, int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null || book.IsAvailable)
                throw new Exception("Book is not available for reservation");

            var reservation = new Reservation
            {
                UserId = userId,
                BookId = bookId,
                ReservationDate = DateTime.UtcNow
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return reservation;
        }

        public async Task CancelReservationAsync(int reservationId)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);
            if (reservation == null)
                throw new Exception("Reservation not found");

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
        }

        private string CreatePasswordHash(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            return storedHash == CreatePasswordHash(password);
        }
    }
}