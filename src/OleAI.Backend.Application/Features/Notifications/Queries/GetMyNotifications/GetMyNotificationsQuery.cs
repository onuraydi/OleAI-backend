using MediatR;
using OleAI.Backend.Application.Features.Notifications.Dtos;
using OleAI.Backend.Core.Repositories.Pagination;

namespace OleAI.Backend.Application.Features.Notifications.Queries.GetMyNotifications
{
    public class GetMyNotificationsQuery : IRequest<Paginate<NotificationDto>>
    {
        public int Page { get; set; } = 0;
        public int Size { get; set; } = 10;
        public bool OnlyUnread { get; set; } = false;
    }
}
