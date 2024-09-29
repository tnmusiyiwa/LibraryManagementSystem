namespace LibraryManagement.API.Services
{
    using LibraryManagement.API.Data;
    using LibraryManagement.API.Models;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Defines the <see cref="ReservationService"/>
    /// </summary>
    public class ReservationService : IReservationService
    {
        /// <summary>
        /// Defines the _context
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Defines the _notificationService
        /// </summary>
        private readonly INotificationService _notificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReservationService"/>
        /// class.
        /// </summary>
        /// <param name="context">The context<see cref="ApplicationDbContext"/>
        ///     </param>
        /// <param name="notificationService">The
        ///     notificationService<see cref="INotificationService"/></param>
        public ReservationService(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        /// <summary>
        /// The AddReservationAsync
        /// </summary>
        /// <param name="reservation">The reservation<see cref="Reservation"/>
        ///     </param>
        /// <returns>The <see cref="Task{Reservation}"/></returns>
        public async Task<Reservation> AddReservationAsync(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return reservation;
        }

        /// <summary>
        /// The CancelReservationAsync
        /// </summary>
        /// <param name="reservationId">The reservationId<see cref="int"/>
        ///     </param>
        /// <returns>The <see cref="Task"/></returns>
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

            // Check if there are any notifications for this book
            var oldestNotification = await _context.Notifications
                .Where(n => n.BookId == reservation.BookId && !n.IsSent)
                .OrderByDescending(n => n.CreatedDate)
                .FirstOrDefaultAsync();

            if (oldestNotification != null)
            {
                // Create a notification for the user with oldest reservation
                await _notificationService.SendNotication(oldestNotification.Id);
            }
        }

        /// <summary>
        /// The DeleteReservationAsync
        /// </summary>
        /// <param name="id">The id<see cref="int"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task DeleteReservationAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// The GetAllAsync
        /// </summary>
        /// <returns>The <see cref="Task{IEnumerable{Reservation}}"/></returns>
        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            var query = _context.Reservations.AsQueryable();

            return await query
                .OrderByDescending(r => r.ReservationDate)
                .ToListAsync();
        }

        /// <summary>
        /// The GetReservationByIdAsync
        /// </summary>
        /// <param name="id">The id<see cref="int"/></param>
        /// <returns>The <see cref="Task{Reservation}"/></returns>
        public async Task<Reservation> GetReservationByIdAsync(int id)
        {
            return await _context.Reservations.FindAsync(id);
        }

        /// <summary>
        /// The GetReservationsAsync
        /// </summary>
        /// <param name="userId">The userId<see cref="string"/></param>
        /// <returns>The <see cref="Task{IEnumerable{Reservation}}"/></returns>
        public async Task<IEnumerable<Reservation>> GetReservationsAsync(string userId)
        {
            return await _context.Reservations
                .Where(r => r.UserId == userId)
                .Include(r => r.Book)
            .ToListAsync();
        }

        /// <summary>
        /// The UpdateReservationAsync
        /// </summary>
        /// <param name="reservation">The reservation<see cref="Reservation"/>
        ///     </param>
        /// <returns>The <see cref="Task{Reservation}"/></returns>
        public async Task<Reservation> UpdateReservationAsync(Reservation reservation)
        {
            _context.Entry(reservation).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return reservation;
        }

        public async Task<Reservation> GetReservationByUserByBook(string userId, int bookId)
        {
            return await _context.Reservations
                .Where(r => r.UserId == userId && r.BookId == bookId && !r.IsCanceled)
                .FirstOrDefaultAsync();
        }
    }
}
