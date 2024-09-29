using LibraryManagement.API.Data;
using LibraryManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.API.Services
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly IReservationService _reservationService;

        public BookService(ApplicationDbContext context, INotificationService notificationService, IReservationService reservationService)
        {
            _context = context;
            _notificationService = notificationService;
            _reservationService = reservationService;
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
            var reservation = await _reservationService.GetReservationByUserByBook(userId, bookId);

            if (book == null || (!book.IsAvailable && reservation == null))
                throw new Exception("Book is not available for borrowing");

            var borrowDate = DateTime.UtcNow;
            var dueDate = borrowDate.AddDays(days);

            var borrowedBook = new BorrowedBook
            {
                UserId = userId,
                BookId = bookId,
                BorrowDate = borrowDate,
                DueDate = dueDate
            };

            book.IsAvailable = false;
            _context.BorrowedBooks.Add(borrowedBook);
            await _context.SaveChangesAsync();

            if (reservation != null)
                await _reservationService.DeleteReservationAsync(reservation.Id);

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

            // Create a notification for the user returning book
            await _notificationService.CreateNotificationAsync(
                borrowedBook.UserId,
                $"You have successfully returned the book '{borrowedBook.Book.Title}' on {borrowedBook.ReturnDate:d}.",
                borrowedBook.Book.Id
            );

            // Check if there are any notifications for this book
            var oldestNotification = await _context.Notifications
                .Where(n => n.BookId == borrowedBook.BookId && !n.IsSent)
                .OrderByDescending(n => n.CreatedDate)
                .FirstOrDefaultAsync();

            if (oldestNotification != null)
            {
                // Create a notification for the user with oldest reservation
                await _notificationService.SendNotication(oldestNotification.Id);
            }
        }

        public async Task<Reservation> ReserveBookAsync(string userId, int bookId, bool notifyWhenAvailable = false)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null || !book.IsAvailable)
            {
                if (notifyWhenAvailable && book != null)
                {
                    // Create a notification to be notified when book
                    await _notificationService.CreateNotificationAsync(
                        userId,
                        $"The book '{book.Title}' you reserved is now available. Please visit the library to borrow it.",
                        bookId,
                        false
                    );
                }

                throw new Exception("Book is not available for reservation");
            }

            var reservation = new Reservation
            {
                UserId = userId,
                BookId = bookId,
                ReservationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(1)
            };

            _context.Reservations.Add(reservation);

            book.IsAvailable = false;

            await _context.SaveChangesAsync();

            return reservation;
        }

        public async Task<IEnumerable<BorrowedBook>> GetBorrowedBooksAsync()
        {
            return await _context.BorrowedBooks
                .Include(bb => bb.Book)
                .Include(bb => bb.User)
                .Where(bb => bb.ReturnDate == null)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetReservedBooksAsync()
        {
            return await _context.Reservations
                .Include(r => r.Book)
                .Include(r => r.User)
                .Where(r => !r.IsCanceled)
                .ToListAsync();
        }

        public async Task<IEnumerable<BorrowedBook>> GetOverdueBooksAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _context.BorrowedBooks
                .Include(bb => bb.Book)
                .Include(bb => bb.User)
                .Where(bb => bb.ReturnDate == null && bb.DueDate < today)
                .ToListAsync();
        }

        public async Task<IEnumerable<BorrowedBook>> GetAlmostDueBooksAsync()
        {
            var today = DateTime.UtcNow.Date;
            var threeDaysFromNow = today.AddDays(3);
            return await _context.BorrowedBooks
                .Include(bb => bb.Book)
                .Include(bb => bb.User)
                .Where(bb => bb.ReturnDate == null && bb.DueDate >= today && bb.DueDate <= threeDaysFromNow)
                .ToListAsync();
        }
    }
}