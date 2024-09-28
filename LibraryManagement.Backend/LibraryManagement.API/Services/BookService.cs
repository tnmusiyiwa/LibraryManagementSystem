using LibraryManagement.API.Data;
using LibraryManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.API.Services
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public BookService(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync(int page, int pageSize, string searchQuery)
        {
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(b =>
                    b.Title.Contains(searchQuery) ||
                    b.Author.Contains(searchQuery) ||
                    b.Category.Contains(searchQuery) ||
                    b.ISBN.Contains(searchQuery)
                );
            }

            return await query
                .OrderBy(b => b.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Book> GetBookByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task<Book> AddBookAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<Book> UpdateBookAsync(Book book)
        {
            _context.Entry(book).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetTotalBooksCountAsync(string searchQuery)
        {
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(b =>
                    b.Title.Contains(searchQuery) ||
                    b.Author.Contains(searchQuery) ||
                    b.Category.Contains(searchQuery) ||
                    b.ISBN.Contains(searchQuery)
                );
            }

            return await query.CountAsync();
        }

        public async Task ReturnBookAsync(int borrowedBookId)
        {
            var borrowedBook = await _context.BorrowedBooks
                .Include(bb => bb.Book)
                .Include(bb => bb.User)
                .FirstOrDefaultAsync(bb => bb.Id == borrowedBookId);

            if (borrowedBook == null)
            {
                throw new ArgumentException("Borrowed book not found!");
            }

            borrowedBook.ReturnDate = DateTime.UtcNow;
            borrowedBook.Book.IsAvailable = true;

            await _context.SaveChangesAsync();

            // Create a notification for the user returning book
            await _notificationService.CreateNotificationAsync(
                borrowedBook.UserId,
                $"You have successfully returned the book '{borrowedBook.Book.Title}' on {borrowedBook.ReturnDate:d}.",
                borrowedBook.Book.Id
            );

            // Check if there are any reservations for this book
            var oldestReservation = await _context.Reservations
                .Where(r => r.BookId == borrowedBook.BookId && !r.IsCanceled && r.ExpiryDate >= DateTime.UtcNow)
                .OrderBy(r => r.ReservationDate)
                .FirstOrDefaultAsync();

            if (oldestReservation != null)
            {
                // Create a notification for the user with oldest reservation
                await _notificationService.CreateNotificationAsync(
                    oldestReservation.UserId,
                    $"The book '{borrowedBook.Book.Title}' you reserved is now available. Please visit the library to borrow it.",
                    oldestReservation.Book.Id
                );
            }
        }

        public async Task CancelReservationAsync(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Book)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null)
            {
                throw new ArgumentException("Reservation not found!");
            }

            reservation.IsCanceled = true;
            await _context.SaveChangesAsync();

            // Create a notification for the user
            await _notificationService.CreateNotificationAsync(
                reservation.UserId,
                $"Your reservation for the book '{reservation.Book.Title}' has been canceled.",
                reservation.BookId
            );

            // Check if there any other reservations for this book
            var nextReservation = await _context.Reservations
                .Where(r => r.BookId == reservation.BookId && !r.IsCanceled && r.Id != reservationId && r.ExpiryDate > DateTime.UtcNow)
                .OrderBy(r => r.ReservationDate)
                .FirstOrDefaultAsync();

            if (nextReservation != null)
            {
                // Create a notification for the user with the next reservation
                await _notificationService.CreateNotificationAsync(
                    nextReservation.UserId,
                    $"A reservation for the book '{reservation.Book.Title}' has been canceled. Your reservation has moved up in the queue.",
                    nextReservation.BookId
                );
            }
        }

    }
}