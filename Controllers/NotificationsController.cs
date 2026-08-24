using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskMngBack.DTOs.Notifications;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<ActionResult<List<NotificationDto>>> GetForUser()
        {
            var notifications = await _notificationService.GetForUserAsync(GetCurrentUserId());
            return Ok(notifications);
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            var count = await _notificationService.GetUnreadCountAsync(GetCurrentUserId());
            return Ok(count);
        }

        [HttpPut("{id:guid}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            await _notificationService.MarkAsReadAsync(id, GetCurrentUserId());
            return NoContent();
        }

        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await _notificationService.MarkAllAsReadAsync(GetCurrentUserId());
            return NoContent();
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
    }
}
