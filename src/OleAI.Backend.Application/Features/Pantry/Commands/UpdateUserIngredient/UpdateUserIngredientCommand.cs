using MediatR;
using OleAI.Backend.Application.Features.Pantry.Dtos.Requests;

namespace OleAI.Backend.Application.Features.Pantry.Commands.UpdateUserIngredient
{
    public class UpdateUserIngredientCommand : IRequest<string>
    {
        public UpdateUserIngredientRequest Request { get; set; } = default!;
    }
}
