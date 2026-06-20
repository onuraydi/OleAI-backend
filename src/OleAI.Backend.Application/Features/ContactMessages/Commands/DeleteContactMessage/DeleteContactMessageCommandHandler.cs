using AutoMapper;
using MediatR;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.ContactMessages.Commands.DeleteContactMessage
{
    public class DeleteContactMessageCommandHandler : IRequestHandler<DeleteContactMessageCommand, DeletedContactMessageResponse>
    {
        private readonly IContactMessageRepository _contactMessageRepository;
        private readonly IMapper _mapper;

        public DeleteContactMessageCommandHandler(IContactMessageRepository contactMessageRepository, IMapper mapper)
        {
            _contactMessageRepository = contactMessageRepository;
            _mapper = mapper;
        }

        public async Task<DeletedContactMessageResponse> Handle(DeleteContactMessageCommand request, CancellationToken cancellationToken)
        {
            var contactMessage = await _contactMessageRepository.GetAsync(predicate: c => c.Id == request.Id, cancellationToken: cancellationToken);

            if (contactMessage == null)
            {
                throw new Exception("Contact Message not found");
            }

            await _contactMessageRepository.DeleteAsync(contactMessage);

            return _mapper.Map<DeletedContactMessageResponse>(contactMessage);
        }
    }
}
