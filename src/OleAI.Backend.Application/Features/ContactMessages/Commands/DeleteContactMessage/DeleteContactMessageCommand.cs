using MediatR;

namespace OleAI.Backend.Application.Features.ContactMessages.Commands.DeleteContactMessage
{
    public class DeleteContactMessageCommand : IRequest<DeletedContactMessageResponse>
    {
        public Guid Id { get; set; }
    }
}
