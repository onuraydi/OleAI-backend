using MediatR;

namespace OleAI.Backend.Application.Features.Notifications.Commands.MarkNotificationAsRead
{
    public class MarkNotificationAsReadCommand : IRequest<bool>
    {
        public Guid NotificationId { get; set; }
    }
}
