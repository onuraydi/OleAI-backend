using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.Notifications.Commands.MarkNotificationAsRead
{
    public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, bool>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MarkNotificationAsReadCommandHandler(INotificationRepository notificationRepository, IHttpContextAccessor httpContextAccessor)
        {
            _notificationRepository = notificationRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId)) return false;
            var notification = await _notificationRepository.GetAsync(n => n.Id == request.NotificationId && n.UserId == userId, cancellationToken: cancellationToken);

            if (notification == null)
            {
                return false;
            }

            if (!notification.IsRead)
            {
                notification.MarkAsRead();
                await _notificationRepository.UpdateAsync(notification);
            }

            return true;
        }
    }
}
