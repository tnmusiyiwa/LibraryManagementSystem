using LibraryManagement.API.Models;

namespace LibraryManagement.API.Services
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(string userId, string message, int? bookId);
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId);
        Task MarkNotificationAsSentAsync(int notificationId);
        Task DeleteNotificationAsync(int notificationId);
        Task SendNofications();
    }
}
