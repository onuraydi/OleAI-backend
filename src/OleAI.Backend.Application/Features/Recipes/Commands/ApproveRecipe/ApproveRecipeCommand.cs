using MediatR;

namespace OleAI.Backend.Application.Features.Recipes.Commands.ApproveRecipe;

public class ApproveRecipeCommand : IRequest<string>
{
    public Guid Id { get; set; }
}
