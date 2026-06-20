using System.Security.Claims;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using OleAI.Backend.Application.Features.Notifications.Dtos;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Notifications.Queries.GetMyNotifications
{
    public class GetMyNotificationsQueryHandler : IRequestHandler<GetMyNotificationsQuery, Paginate<NotificationDto>>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public GetMyNotificationsQueryHandler(INotificationRepository notificationRepository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<Paginate<NotificationDto>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
        {
            var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId)) return new Paginate<NotificationDto>();

            QueryOptions<Notification> queryOptions = new QueryOptions<Notification>
            {
                Predicate = n => n.UserId == userId && (!request.OnlyUnread || !n.IsRead),
                OrderBy = query => query.OrderByDescending(n => n.CreatedDate),
                Index = request.Page,
                Size = request.Size,
                EnableTracking = false
            };

            var notifications = await _notificationRepository.GetListAsync(queryOptions, cancellationToken);
            return _mapper.Map<Paginate<NotificationDto>>(notifications);
        }
    }
}
