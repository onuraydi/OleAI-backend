using MediatR;

namespace OleAI.Backend.Application.Features.Recipes.Commands.StartCooking;

public class StartCookingCommand : IRequest<string>
{
    public Guid RecipeId { get; set; }
}
