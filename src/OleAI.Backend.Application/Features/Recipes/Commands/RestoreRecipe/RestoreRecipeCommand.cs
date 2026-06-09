using MediatR;

namespace OleAI.Backend.Application.Features.Recipes.Commands.RestoreRecipe;

public class RestoreRecipeCommand : IRequest<string>
{
    public Guid Id { get; set; }
}
