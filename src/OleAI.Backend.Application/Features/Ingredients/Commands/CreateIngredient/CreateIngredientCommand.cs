using MediatR;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Requests;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Responses;

namespace OleAI.Backend.Application.Features.Ingredients.Commands.CreateIngredient
{
    public class CreateIngredientCommand : IRequest<CreatedIngredientResponse>
    {
        public CreateIngredientRequest Request { get; set; } = default!;
    }
}
