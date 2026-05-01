using MediatR;
using OleAI.Backend.Application.Features.Recipes.Dtos.Requests;

namespace OleAI.Backend.Application.Features.Recipes.Commands.CreateRecipe
{
    public class CreateRecipeCommand : IRequest<string>
    {
        public CreateRecipeRequest CreateRecipeRequest { get; set; } = default!;
        public List<CreateRecipeIngredientRequest> Ingredients { get; set; } = new();
        public List<CreateRecipeStepRequest> Steps { get; set; } = new();
    }
}
