using MediatR;

namespace OleAI.Backend.Application.Features.Notifications.Commands.DeleteNotification
{
    public class DeleteNotificationCommand : IRequest<bool>
    {
        public Guid NotificationId { get; set; }
    }
}
