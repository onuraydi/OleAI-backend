using MediatR;

namespace OleAI.Backend.Application.Features.Recipes.Commands.HardDeleteRecipe;

public class HardDeleteRecipeCommand : IRequest<string>
{
    public Guid Id { get; set; }
}
