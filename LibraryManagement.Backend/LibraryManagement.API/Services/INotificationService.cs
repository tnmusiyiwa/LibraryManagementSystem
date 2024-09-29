using LibraryManagement.API.Models;

namespace LibraryManagement.API.Services
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(string userId, string message, int? bookId, bool sendImmediately = true);
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId);
        Task MarkNotificationAsSentAsync(int notificationId);
        Task DeleteNotificationAsync(int notificationId);
        Task SendNofications();
        Task SendNotication(int notificationId);
        Task<IEnumerable<Notification>> GetUnsentNotificationsAsync();
    }
}
