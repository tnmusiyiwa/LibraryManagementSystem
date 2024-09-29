using LibraryManagement.API.Models;
using LibraryManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace LibraryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly INotificationService _notificationService;
        private readonly IReservationService _reservationService;

        public AdminController(IBookService bookService, INotificationService notificationService, IReservationService reservationService)
        {
            _bookService = bookService;
            _notificationService = notificationService;
            _reservationService = reservationService;
        }

        [HttpGet("borrowed-books")]
        [SwaggerOperation("GetBorrowedBooks")]
        [SwaggerResponse(statusCode: 200, type: typeof(IEnumerable<BorrowedBook>), description: "Get all borrowed books")]
        public async Task<IActionResult> GetBorrowedBooks()
        {
            var borrowedBooks = await _bookService.GetBorrowedBooksAsync();
            return Ok(borrowedBooks);
        }

        [HttpGet("reserved-books")]
        [SwaggerOperation("GetReservedBooks")]
        [SwaggerResponse(statusCode: 200, type: typeof(IEnumerable<Reservation>), description: "Get all reserved books")]

        public async Task<IActionResult> GetReservedBooks()
        {
            var reservedBooks = await _reservationService.GetReservedBooksAsync();
            return Ok(reservedBooks);
        }

        [HttpGet("overdue-books")]
        [SwaggerOperation("GetOverdueBooks")]
        [SwaggerResponse(statusCode: 200, type: typeof(IEnumerable<BorrowedBook>), description: "Get all borrowed books that are now overdue")]

        public async Task<IActionResult> GetOverdueBooks()
        {
            var overdueBooks = await _bookService.GetOverdueBooksAsync();
            return Ok(overdueBooks);
        }

        [HttpGet("almost-due-books")]
        [SwaggerOperation("GetAlmostDueBooks")]
        [SwaggerResponse(statusCode: 200, type: typeof(IEnumerable<BorrowedBook>), description: "Get all borrowed books that are near return date")]

        public async Task<IActionResult> GetAlmostDueBooks()
        {
            var almostDueBooks = await _bookService.GetAlmostDueBooksAsync();
            return Ok(almostDueBooks);
        }

        [HttpGet("unsent-notifications")]
        [SwaggerOperation("GetUnsentNotifications")]
        [SwaggerResponse(statusCode: 200, type: typeof(IEnumerable<Notification>), description: "Get notifications that are not yet sent to get user interests")]

        public async Task<IActionResult> GetUnsentNotifications()
        {
            var unsentNotifications = await _notificationService.GetUnsentNotificationsAsync();
            return Ok(unsentNotifications);
        }
    }
}