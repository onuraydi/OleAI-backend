using MediatR;

using OleAI.Backend.Application.Features.Ingredients.Dtos.Responses;

namespace OleAI.Backend.Application.Features.Recipes.Commands.StartCooking;

public class StartCookingCommand : IRequest<List<IngredientResponse>>
{
    public Guid RecipeId { get; set; }
    public List<Guid> IgnoredIngredientIds { get; set; } = new();
}
