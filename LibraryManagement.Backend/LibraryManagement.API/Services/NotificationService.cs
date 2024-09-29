using LibraryManagement.API.Data;
using LibraryManagement.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateNotificationAsync(string userId, string message, int? bookId = null, bool sendImmediately = true)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                IsSent = false,
                CreatedDate = DateTime.UtcNow,
            };

            if (bookId != null)
            {
                notification.BookId = bookId;
            }

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            if (sendImmediately)
            {
                await this.SendNotication(notification.Id);
            }
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsSent)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task MarkNotificationAsSentAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsSent = true;
                notification.SentDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteNotificationAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SendNotication(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                // TODO: Logic to send notification to user

                await this.MarkNotificationAsSentAsync(notificationId);
            }
        }

        public async Task SendNofications()
        {
            var notifications = await _context.Notifications.Where(n => !n.IsSent).ToListAsync();

            foreach (var notification in notifications)
            {
                // TODO: Add notification sending logic

                await this.MarkNotificationAsSentAsync(notification.Id);
            }
        }
    }
}