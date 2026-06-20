using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.Notifications.Commands.DeleteNotification
{
    public class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommand, bool>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteNotificationCommandHandler(INotificationRepository notificationRepository, IHttpContextAccessor httpContextAccessor)
        {
            _notificationRepository = notificationRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
        {
            var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"DeleteNotification: Request NotificationId: {request.NotificationId}, UserIdStr: {userIdStr}");

            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            {
                Console.WriteLine("DeleteNotification: UserId parsing failed.");
                return false;
            }

            var notification = await _notificationRepository.GetAsync(n => n.Id == request.NotificationId && n.UserId == userId, cancellationToken: cancellationToken);

            if (notification == null)
            {
                Console.WriteLine($"DeleteNotification: Notification not found in DB. UserId: {userId}, NotifId: {request.NotificationId}");
                return false;
            }

            await _notificationRepository.DeleteAsync(notification);
            Console.WriteLine("DeleteNotification: Notification successfully deleted.");
            return true;
        }
    }
}
