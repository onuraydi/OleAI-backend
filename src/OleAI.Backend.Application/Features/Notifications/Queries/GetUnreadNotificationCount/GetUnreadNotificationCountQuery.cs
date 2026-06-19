using MediatR;

namespace OleAI.Backend.Application.Features.Notifications.Queries.GetUnreadNotificationCount
{
    public class GetUnreadNotificationCountQuery : IRequest<int>
    {
    }
}
