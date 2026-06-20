using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OleAI.Backend.Application.Features.Notifications.Commands.DeleteNotification;
using OleAI.Backend.Application.Features.Notifications.Commands.MarkAllNotificationsAsRead;
using OleAI.Backend.Application.Features.Notifications.Commands.MarkNotificationAsRead;
using OleAI.Backend.Application.Features.Notifications.Queries.GetMyNotifications;
using OleAI.Backend.Application.Features.Notifications.Queries.GetUnreadNotificationCount;
using OleAI.Backend.Application.Services.PushNotifications;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetMyNotifications([FromQuery] int page = 0, [FromQuery] int size = 10, [FromQuery] bool onlyUnread = false)
        {
            var query = new GetMyNotificationsQuery { Page = page, Size = size, OnlyUnread = onlyUnread };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var query = new GetUnreadNotificationCountQuery();
            var result = await Mediator.Send(query);
            return Ok(new { Count = result });
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead([FromRoute] Guid id)
        {
            var command = new MarkNotificationAsReadCommand { NotificationId = id };
            var result = await Mediator.Send(command);

            if (!result) return NotFound();

            return Ok();
        }

        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var command = new MarkAllNotificationsAsReadCommand();
            await Mediator.Send(command);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var command = new DeleteNotificationCommand { NotificationId = id };
            var result = await Mediator.Send(command);

            if (!result) return NotFound();

            return Ok();
        }

        [HttpPost("test-trigger")]
        public async Task<IActionResult> TriggerTestNotification(
            [FromServices] INotificationRepository notificationRepository,
            [FromServices] IUserDeviceRepository userDeviceRepository,
            [FromServices] IPushNotificationService pushNotificationService)
        {
            var userIdStr = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

            var notification = new Notification(
                Guid.NewGuid(),
                userId,
                "Test Bildirimi 🚀",
                "Bu bir test bildirimidir. Firebase entegrasyonu başarıyla çalışıyor demektir!",
                NotificationType.System,
                null
            );

            await notificationRepository.AddAsync(notification);

            var devices = await userDeviceRepository.GetListAsync(
                new OleAI.Backend.Core.Repositories.Options.QueryOptions<UserDevice>
                {
                    Predicate = d => d.UserId == userId
                });

            var tokens = devices.Items.Select(d => d.DeviceToken).ToList();
            if (tokens.Any())
            {
                await pushNotificationService.SendMulticastPushNotificationAsync(
                    tokens,
                    notification.Title,
                    notification.Message
                );
            }

            return Ok(new { Message = "Test bildirimi gönderildi", TokensFound = tokens.Count });
        }

        [HttpPost("send-global")]
        // [Authorize(Roles = "Admin")] // Uncomment when roles are implemented
        public async Task<IActionResult> SendGlobalNotification(
            [FromBody] string message,
            [FromServices] INotificationRepository notificationRepository,
            [FromServices] IUserDeviceRepository userDeviceRepository,
            [FromServices] IPushNotificationService pushNotificationService)
        {
            var adminUserIdStr = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminUserIdStr) || !Guid.TryParse(adminUserIdStr, out var adminUserId)) return Unauthorized();

            // Tüm cihazları bul (örnek amaçlı, gerçekte sayfalı veya tüm kullanıcılara loop)
            var allDevices = await userDeviceRepository.GetListAsync(
                new OleAI.Backend.Core.Repositories.Options.QueryOptions<UserDevice>
                {
                    Size = 10000
                });

            var tokens = allDevices.Items.Select(d => d.DeviceToken).Distinct().ToList();

            // Tüm kullanıcılara Notification tablosuna kayıt at
            var distinctUserIds = allDevices.Items.Select(d => d.UserId).Distinct().ToList();
            var notifications = distinctUserIds.Select(uId => new Notification(
                Guid.NewGuid(),
                uId,
                "Sistem Duyurusu 📣",
                message,
                NotificationType.System,
                null
            )).ToList();

            if (notifications.Any())
            {
                await notificationRepository.AddRangeAsync(notifications);
            }

            if (tokens.Any())
            {
                await pushNotificationService.SendMulticastPushNotificationAsync(
                    tokens,
                    "Sistem Duyurusu 📣",
                    message
                );
            }

            return Ok(new { Message = "Global bildirim başarıyla gönderildi", UsersReached = distinctUserIds.Count, DevicesReached = tokens.Count });
        }
    }
}
