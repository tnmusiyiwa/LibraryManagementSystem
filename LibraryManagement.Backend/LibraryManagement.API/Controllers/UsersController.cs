using LibraryManagement.API.Attributes;
using LibraryManagement.API.Dtos;
using LibraryManagement.API.Models;
using LibraryManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
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
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;
        private readonly IBookService _bookService;
        private readonly IReservationService _reservationService;

        public UsersController(
            IUserService userService,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager,
            ILogger<UsersController> logger,
            IBookService bookService,
            IReservationService reservationService)
        {
            _userService = userService;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
            _bookService = bookService;
            _reservationService = reservationService;
        }

        [HttpPost("register")]
        [ValidateModelState]
        [SwaggerOperation("Register")]
        [SwaggerResponse(statusCode: 200, type: typeof(ApplicationUser), description: "Register as a new user")]

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
        [ValidateModelState]
        [SwaggerOperation("Login")]
        [SwaggerResponse(statusCode: 200, type: typeof(object), description: "Login user returns token and expiry date")]

        public async Task<IActionResult> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("assign-role")]
        [ValidateModelState]
        [SwaggerOperation("AssignRole")]
        [SwaggerResponse(statusCode: 200, type: typeof(string), description: "Assign user to a role")]

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
        [ValidateModelState]
        [SwaggerOperation("GetCurrentUser")]
        [SwaggerResponse(statusCode: 200, type: typeof(ApplicationUser), description: "Get current user details")]

        public async Task<IActionResult> GetCurrentUser()
        {
            _logger.LogInformation("GetCurrentUser called. User.Identity.Name: {Name}", User.Identity?.Name);
            _logger.LogInformation("User.Identity.IsAuthenticated: {IsAuthenticated}", User.Identity?.IsAuthenticated);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("User not found in database");
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new { user.Id, user.Email, user.Name, Roles = roles });
        }

        [HttpGet("{id}")]
        [Authorize]
        [ValidateModelState]
        [SwaggerOperation("GetUser")]
        [SwaggerResponse(statusCode: 200, type: typeof(ApplicationUser), description: "Get user by Id")]

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
        [SwaggerOperation("GetBorrowedBooks")]
        [SwaggerResponse(statusCode: 200, type: typeof(IEnumerable<BorrowedBook>), description: "Get user's borrowed books")]

        public async Task<ActionResult<IEnumerable<BorrowedBook>>> GetBorrowedBooks()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var borrowedBooks = await _bookService.GetBorrowedBooksAsync(userId);
            return Ok(borrowedBooks);
        }

        [HttpGet("reservations")]
        [Authorize]
        [ValidateModelState]
        [SwaggerOperation("GetReservations")]
        [SwaggerResponse(statusCode: 200, type: typeof(IEnumerable<Reservation>), description: "Get user's reservations")]

        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var reservations = await _bookService.GetReservationsAsync(userId);
            return Ok(reservations);
        }

        [HttpPost("borrow")]
        [Authorize]
        [ValidateModelState]
        [SwaggerOperation("BorrowBook")]
        [SwaggerResponse(statusCode: 200, type: typeof(BorrowedBook), description: "Borrow a book")]

        public async Task<ActionResult<BorrowedBook>> BorrowBook(BorrowBookDto borrowBookDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var borrowedBook = await _bookService.BorrowBookAsync(userId, borrowBookDto.BookId, borrowBookDto.Days);
                return Ok(borrowedBook);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("return")]
        [Authorize]
        [ValidateModelState]
        [SwaggerOperation("ReturnBook")]
        [SwaggerResponse(statusCode: 200, type: typeof(void), description: "Return a book")]

        public async Task<IActionResult> ReturnBook(ReturnBookDto returnBookDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                await _bookService.ReturnBookAsync(userId, returnBookDto.BookId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("reserve")]
        [Authorize]
        [ValidateModelState]
        [SwaggerOperation("ReserveBook")]
        [SwaggerResponse(statusCode: 200, type: typeof(Reservation), description: "Reserve book")]

        public async Task<ActionResult<Reservation>> ReserveBook(ReserveBookDto reserveBookDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var reservation = await _bookService.ReserveBookAsync(userId, reserveBookDto.BookId, reserveBookDto.NotifyWhenAvailable);
                return Ok(reservation);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("reservations/{id}")]
        [Authorize]
        [ValidateModelState]
        [SwaggerOperation("CancelReservation")]
        [SwaggerResponse(statusCode: 200, type: typeof(void), description: "Cancel reservation")]

        public async Task<IActionResult> CancelReservation(int id)
        {
            try
            {
                await _reservationService.CancelReservationAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ValidateModelState]
        [SwaggerOperation("GetAllUsers")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<ApplicationUser>), description: "Get all users (admin only)")]

        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users
                .Select(u => new { u.Id, u.Email, u.Name, u.UserName })
                .ToListAsync();
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateModelState]
        [SwaggerOperation("CreateUser")]
        [SwaggerResponse(statusCode: 200, type: typeof(string), description: "Create a new user (admin only)")]

        public async Task<IActionResult> CreateUser(CreateUserDto model)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email, Name = model.Name };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);
                return Ok(new { message = "User created successfully" });
            }

            return BadRequest(result.Errors);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ValidateModelState]
        [SwaggerOperation("UpdateUser")]
        [SwaggerResponse(statusCode: 200, type: typeof(string), description: "Update user details (admin only)")]

        public async Task<IActionResult> UpdateUser(string id, UpdateUserDto model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Email = model.Email;
            user.UserName = model.Email;
            user.Name = model.Name;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, model.Role);
                return Ok(new { message = "User updated successfully" });
            }

            return BadRequest(result.Errors);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ValidateModelState]
        [SwaggerOperation("DeleteUser")]
        [SwaggerResponse(statusCode: 200, type: typeof(string), description: "Delete a user (admin only)")]

        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return Ok(new { message = "User deleted successfully" });
            }

            return BadRequest(result.Errors);
        }
    }
}