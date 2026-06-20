using AutoMapper;
using MediatR;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.ContactMessages.Queries.GetContactMessages
{
    public class GetContactMessagesQuery : IRequest<Paginate<GetContactMessageListItemDto>>
    {
        public int Page { get; set; } = 0;
        public int Size { get; set; } = 10;

        public class GetContactMessagesQueryHandler : IRequestHandler<GetContactMessagesQuery, Paginate<GetContactMessageListItemDto>>
        {
            private readonly IContactMessageRepository _contactMessageRepository;
            private readonly IMapper _mapper;

            public GetContactMessagesQueryHandler(IContactMessageRepository contactMessageRepository, IMapper mapper)
            {
                _contactMessageRepository = contactMessageRepository;
                _mapper = mapper;
            }

            public async Task<Paginate<GetContactMessageListItemDto>> Handle(GetContactMessagesQuery request, CancellationToken cancellationToken)
            {
                var queryOptions = new QueryOptions<ContactMessage>
                {
                    Index = request.Page,
                    Size = request.Size,
                    OrderBy = x => x.OrderByDescending(c => c.CreatedDate)
                };

                Paginate<ContactMessage> contactMessages = await _contactMessageRepository.GetListAsync(
                    queryOptions,
                    cancellationToken: cancellationToken
                );

                Paginate<GetContactMessageListItemDto> mappedContactMessageListModel = _mapper.Map<Paginate<GetContactMessageListItemDto>>(contactMessages);
                return mappedContactMessageListModel;
            }
        }
    }
}
