using MediatR;
using OleAI.Backend.Application.Features.Pantry.Dtos.Requests;

namespace OleAI.Backend.Application.Features.Pantry.Commands.ResolveRequest
{
    public class ResolveIngredientRequestCommand : IRequest<string>
    {
        public ResolveIngredientRequestDto Request { get; set; } = default!;
    }
}
