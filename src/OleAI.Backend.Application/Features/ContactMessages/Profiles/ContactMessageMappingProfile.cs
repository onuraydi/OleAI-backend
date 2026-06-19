using AutoMapper;
using OleAI.Backend.Application.Features.ContactMessages.Commands.CreateContactMessage;
using OleAI.Backend.Application.Features.ContactMessages.Commands.DeleteContactMessage;
using OleAI.Backend.Application.Features.ContactMessages.Queries.GetContactMessages;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.ContactMessages.Profiles
{
    public class ContactMessageMappingProfile : Profile
    {
        public ContactMessageMappingProfile()
        {
            CreateMap<ContactMessage, CreateContactMessageResponse>();
            CreateMap<ContactMessage, GetContactMessageListItemDto>();
            CreateMap<Paginate<ContactMessage>, Paginate<GetContactMessageListItemDto>>();
            CreateMap<ContactMessage, DeletedContactMessageResponse>();
        }
    }
}
