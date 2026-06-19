using MediatR;

namespace OleAI.Backend.Application.Features.Notifications.Commands.MarkAllNotificationsAsRead
{
    public class MarkAllNotificationsAsReadCommand : IRequest<bool>
    {
    }
}
