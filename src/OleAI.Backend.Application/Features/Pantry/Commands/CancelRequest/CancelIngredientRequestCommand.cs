using MediatR;

namespace OleAI.Backend.Application.Features.Pantry.Commands.CancelRequest
{
    public class CancelIngredientRequestCommand : IRequest<string>
    {
        public Guid RequestId { get; set; }
        public Guid UserId { get; set; }
    }
}
