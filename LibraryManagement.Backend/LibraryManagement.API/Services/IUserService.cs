using LibraryManagement.API.Models;

namespace LibraryManagement.API.Services
{
    public interface IUserService
    {
        Task<ApplicationUser> AuthenticateAsync(string email, string password);
        Task<ApplicationUser> RegisterAsync(ApplicationUser user, string password);
        Task<ApplicationUser> GetUserByIdAsync(string id);
        Task<IEnumerable<BorrowedBook>> GetBorrowedBooksAsync(string userId);
        Task<IEnumerable<Reservation>> GetReservationsAsync(string userId);
        Task<BorrowedBook> BorrowBookAsync(string userId, int bookId, int days);
        Task ReturnBookAsync(string userId, int bookId);
        Task<Reservation> ReserveBookAsync(string userId, int bookId);
        Task CancelReservationAsync(int reservationId);
    }
}