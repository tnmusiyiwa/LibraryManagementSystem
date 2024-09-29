using LibraryManagement.API.Models;
using LibraryManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IEnumerable<Reservation>> GetAllReservations()
        {
            return await _reservationService.GetAllAsync();
        }

        [HttpGet("active")]
        [Authorize(Roles = "Admin")]
        public async Task<IEnumerable<Reservation>> GetAllActiveReservations()
        {
            return await _reservationService.GetAllAsync();
        }
    }
}
