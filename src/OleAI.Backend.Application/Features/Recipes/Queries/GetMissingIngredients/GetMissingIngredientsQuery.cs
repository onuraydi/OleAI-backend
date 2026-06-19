using MediatR;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Responses;

namespace OleAI.Backend.Application.Features.Recipes.Queries.GetMissingIngredients
{
    public class GetMissingIngredientsQuery : IRequest<List<IngredientResponse>>
    {
        public Guid RecipeId { get; set; }
    }
}
