using AutoMapper;
using MediatR;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.ContactMessages.Commands.CreateContactMessage
{
    public class CreateContactMessageCommand : IRequest<CreateContactMessageResponse>
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public class CreateContactMessageCommandHandler : IRequestHandler<CreateContactMessageCommand, CreateContactMessageResponse>
        {
            private readonly IContactMessageRepository _contactMessageRepository;
            private readonly IMapper _mapper;

            public CreateContactMessageCommandHandler(IContactMessageRepository contactMessageRepository, IMapper mapper)
            {
                _contactMessageRepository = contactMessageRepository;
                _mapper = mapper;
            }

            public async Task<CreateContactMessageResponse> Handle(CreateContactMessageCommand request, CancellationToken cancellationToken)
            {
                var contactMessage = new ContactMessage
                {
                    Name = request.Name,
                    Email = request.Email,
                    Department = request.Department,
                    Message = request.Message,
                    IsRead = false
                };

                ContactMessage createdMessage = await _contactMessageRepository.AddAsync(contactMessage);

                CreateContactMessageResponse response = _mapper.Map<CreateContactMessageResponse>(createdMessage);
                return response;
            }
        }
    }
}
