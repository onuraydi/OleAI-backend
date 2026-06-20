using AutoMapper;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Notifications.Profiles
{
    public class NotificationMappingProfile : Profile
    {
        public NotificationMappingProfile()
        {
            CreateMap<Notification, Dtos.NotificationDto>();
            CreateMap<Paginate<Notification>, Paginate<Dtos.NotificationDto>>();
        }
    }
}
