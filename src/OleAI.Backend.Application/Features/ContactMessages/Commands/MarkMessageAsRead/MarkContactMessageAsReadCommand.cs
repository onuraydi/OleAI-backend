using MediatR;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.ContactMessages.Commands.MarkMessageAsRead
{
    public class MarkContactMessageAsReadCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public class MarkContactMessageAsReadCommandHandler : IRequestHandler<MarkContactMessageAsReadCommand, bool>
        {
            private readonly IContactMessageRepository _contactMessageRepository;

            public MarkContactMessageAsReadCommandHandler(IContactMessageRepository contactMessageRepository)
            {
                _contactMessageRepository = contactMessageRepository;
            }

            public async Task<bool> Handle(MarkContactMessageAsReadCommand request, CancellationToken cancellationToken)
            {
                ContactMessage? message = await _contactMessageRepository.GetAsync(
                    predicate: c => c.Id == request.Id,
                    cancellationToken: cancellationToken
                );

                if (message == null)
                    throw new Exception("Message not found."); // In a real app we would use BusinessException

                message.IsRead = true;
                await _contactMessageRepository.UpdateAsync(message);

                return true;
            }
        }
    }
}
