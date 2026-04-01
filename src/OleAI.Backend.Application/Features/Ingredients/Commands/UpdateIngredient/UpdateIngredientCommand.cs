using MediatR;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Requests;

namespace OleAI.Backend.Application.Features.Ingredients.Commands.UpdateIngredient
{
    public class UpdateIngredientCommand : IRequest<string>
    {
        public UpdateIngredientRequest Request { get; set; } = default!;
    }
}
