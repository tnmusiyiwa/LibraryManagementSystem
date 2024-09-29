using LibraryManagement.API.Models;

namespace LibraryManagement.API.Services
{
    public interface IReservationService
    {
        Task<IEnumerable<Reservation>> GetAllAsync();
        Task<IEnumerable<Reservation>> GetAllActiveReservationsAsync();
        Task<Reservation> GetReservationByIdAsync(int id);
        Task<Reservation> AddReservationAsync(Reservation reservation);
        Task<Reservation> UpdateReservationAsync(Reservation reservation);
        Task DeleteReservationAsync(int id);
        Task<IEnumerable<Reservation>> GetReservationsAsync(string userId);
        Task CancelReservationAsync(int reservationId);
        Task<Reservation> GetReservationByUserByBook(string userId, int bookId);
        Task<IEnumerable<Reservation>> GetReservedBooksAsync();

    }
}
