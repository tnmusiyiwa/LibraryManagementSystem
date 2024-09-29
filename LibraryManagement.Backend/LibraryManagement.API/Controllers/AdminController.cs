using LibraryManagement.API.Models;
using LibraryManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public AdminController(IBookService bookService, INotificationService notificationService)
        {
            _bookService = bookService;
            _notificationService = notificationService;
        }

        [HttpGet("borrowed-books")]
        public async Task<IActionResult> GetBorrowedBooks()
        {
            var borrowedBooks = await _bookService.GetBorrowedBooksAsync();
            return Ok(borrowedBooks);
        }

        [HttpGet("reserved-books")]
        public async Task<IActionResult> GetReservedBooks()
        {
            var reservedBooks = await _bookService.GetReservedBooksAsync();
            return Ok(reservedBooks);
        }

        [HttpGet("overdue-books")]
        public async Task<IActionResult> GetOverdueBooks()
        {
            var overdueBooks = await _bookService.GetOverdueBooksAsync();
            return Ok(overdueBooks);
        }

        [HttpGet("almost-due-books")]
        public async Task<IActionResult> GetAlmostDueBooks()
        {
            var almostDueBooks = await _bookService.GetAlmostDueBooksAsync();
            return Ok(almostDueBooks);
        }

        [HttpGet("unsent-notifications")]
        public async Task<IActionResult> GetUnsentNotifications()
        {
            var unsentNotifications = await _notificationService.GetUnsentNotificationsAsync();
            return Ok(unsentNotifications);
        }
    }
}