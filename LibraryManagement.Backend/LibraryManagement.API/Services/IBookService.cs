using LibraryManagement.API.Models;

namespace LibraryManagement.API.Services
{
    public interface IBookService
    {
        Task<PaginatedList<Book>> GetPaginatedBooksAsync(int pageIndex, int pageSize);
        Task<IEnumerable<Book>> GetAllBooksAsync(int page, int pageSize, string searchQuery);
        Task<Book> GetBookByIdAsync(int id);
        Task<Book> AddBookAsync(Book book);
        Task<Book> UpdateBookAsync(Book book);
        Task DeleteBookAsync(int id);
        Task<int> GetTotalBooksCountAsync(string searchQuery);
        Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm);
        Task<IEnumerable<BorrowedBook>> GetBorrowedBooksAsync(string userId);
        Task<IEnumerable<Reservation>> GetReservationsAsync(string userId);
        Task<BorrowedBook> BorrowBookAsync(string userId, int bookId, int days);
        Task ReturnBookAsync(string userId, int bookId);
        Task<Reservation> ReserveBookAsync(string userId, int bookId, bool notifyWhenAvailable = false);
        Task<IEnumerable<BorrowedBook>> GetBorrowedBooksAsync();
        Task<IEnumerable<BorrowedBook>> GetOverdueBooksAsync();
        Task<IEnumerable<BorrowedBook>> GetAlmostDueBooksAsync();

    }
}