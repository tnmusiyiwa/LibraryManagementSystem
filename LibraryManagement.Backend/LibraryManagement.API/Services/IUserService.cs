using LibraryManagement.API.Models;

namespace LibraryManagement.API.Services
{
    public interface IUserService
    {
        Task<ApplicationUser> AuthenticateAsync(string email, string password);
        Task<ApplicationUser> RegisterAsync(ApplicationUser user, string password);
        Task<ApplicationUser> GetUserByIdAsync(string id);
    }
}