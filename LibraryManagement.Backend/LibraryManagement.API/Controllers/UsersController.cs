using LibraryManagement.API.Dtos;
using LibraryManagement.API.Models;
using LibraryManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService;

        public UsersController(
            IUserService userService,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userService = userService;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApplicationUser>> Register(RegisterDto model)
        {
            try
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, Name = model.Name };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync("User"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("User"));
                    }

                    await _userManager.AddToRoleAsync(user, "User");
                    return Ok(new { message = "User registered successfully" });
                }

                return BadRequest(result.Errors);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto loginDto)
        {
            var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, false, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                var roles = await _userManager.GetRolesAsync(user);

                return Ok(new { message = "Login successful", roles });
            }

            return Unauthorized(new { message = "Invalid login attempt" });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole(AssignRoleDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                return BadRequest(new { message = "Role does not exist" });
            }

            var result = await _userManager.AddToRoleAsync(user, model.Role);

            if (result.Succeeded)
            {
                return Ok(new { message = "Role assigned successfully" });
            }

            return BadRequest(result.Errors);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new { user.Id, user.Email, Roles = roles });
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ApplicationUser>> GetUser(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpGet("borrowed-books")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<BorrowedBook>>> GetBorrowedBooks()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var borrowedBooks = await _userService.GetBorrowedBooksAsync(userId);
            return Ok(borrowedBooks);
        }

        [HttpGet("reservations")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var reservations = await _userService.GetReservationsAsync(userId);
            return Ok(reservations);
        }

        [HttpPost("borrow")]
        [Authorize]
        public async Task<ActionResult<BorrowedBook>> BorrowBook(BorrowBookDto borrowBookDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var borrowedBook = await _userService.BorrowBookAsync(userId, borrowBookDto.BookId, borrowBookDto.Days);
                return Ok(borrowedBook);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("return")]
        [Authorize]
        public async Task<IActionResult> ReturnBook(ReturnBookDto returnBookDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                await _userService.ReturnBookAsync(userId, returnBookDto.BookId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("reserve")]
        [Authorize]
        public async Task<ActionResult<Reservation>> ReserveBook(ReserveBookDto reserveBookDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var reservation = await _userService.ReserveBookAsync(userId, reserveBookDto.BookId);
                return Ok(reservation);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("reservations/{id}")]
        [Authorize]
        public async Task<IActionResult> CancelReservation(int id)
        {
            try
            {
                await _userService.CancelReservationAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}