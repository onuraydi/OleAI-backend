using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.Notifications.Commands.MarkAllNotificationsAsRead
{
    public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand, bool>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MarkAllNotificationsAsReadCommandHandler(INotificationRepository notificationRepository, IHttpContextAccessor httpContextAccessor)
        {
            _notificationRepository = notificationRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
        {
            var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId)) return false;
            var unreadNotifications = await _notificationRepository.GetListAsync(
                new OleAI.Backend.Core.Repositories.Options.QueryOptions<OleAI.Backend.Domain.Entities.Notification>
                {
                    Predicate = n => n.UserId == userId && !n.IsRead,
                    Size = 1000
                },
                cancellationToken: cancellationToken);

            if (unreadNotifications.Items.Count > 0)
            {
                foreach (var notification in unreadNotifications.Items)
                {
                    notification.MarkAsRead();
                }

                await _notificationRepository.UpdateRangeAsync(unreadNotifications.Items);
            }

            return true;
        }
    }
}
