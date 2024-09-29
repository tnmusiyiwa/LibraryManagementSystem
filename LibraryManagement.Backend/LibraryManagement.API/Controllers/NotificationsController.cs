using LibraryManagement.API.Attributes;
using LibraryManagement.API.Models;
using LibraryManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationsController(INotificationService notificationService, UserManager<ApplicationUser> userManager)
        {
            _notificationService = notificationService;
            _userManager = userManager;
        }

        [HttpGet]
        [SwaggerOperation("GetUserNotification")]
        [SwaggerResponse(statusCode: 200, type: typeof(IEnumerable<Notification>), description: "Get user notifications")]

        public async Task<ActionResult<IEnumerable<Notification>>> GetUserNotifications()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var notifications = await _notificationService.GetUserNotificationsAsync(user.Id);
            return Ok(notifications);
        }

        [HttpPost("{id}/mark-as-sent")]
        [ValidateModelState]
        [SwaggerOperation("MarkNotificationAsSent")]
        [SwaggerResponse(statusCode: 200, type: typeof(NoContent), description: "Mark a notification as sent")]
        public async Task<IActionResult> MarkNotificationAsSent(int id)
        {
            await _notificationService.MarkNotificationAsSentAsync(id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ValidateModelState]
        [SwaggerOperation("DeleteNotification")]
        [SwaggerResponse(statusCode: 200, type: typeof(NoContent), description: "Delete a notification")]

        public async Task<IActionResult> DeleteNotification(int id)
        {
            await _notificationService.DeleteNotificationAsync(id);
            return NoContent();
        }
    }
}