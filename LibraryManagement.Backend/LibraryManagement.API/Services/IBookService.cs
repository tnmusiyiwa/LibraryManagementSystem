using LibraryManagement.API.Models;

namespace LibraryManagement.API.Services
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllBooksAsync(int page, int pageSize, string searchQuery);
        Task<Book> GetBookByIdAsync(int id);
        Task<Book> AddBookAsync(Book book);
        Task<Book> UpdateBookAsync(Book book);
        Task DeleteBookAsync(int id);
        Task<int> GetTotalBooksCountAsync(string searchQuery);
        Task ReturnBookAsync(int borrowedBookId);
    }
}