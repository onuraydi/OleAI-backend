using MediatR;

namespace OleAI.Backend.Application.Features.Recipes.Commands.RejectRecipe;

public class RejectRecipeCommand : IRequest<string>
{
    public Guid Id { get; set; }
    public string Reason { get; set; } = string.Empty;
}
