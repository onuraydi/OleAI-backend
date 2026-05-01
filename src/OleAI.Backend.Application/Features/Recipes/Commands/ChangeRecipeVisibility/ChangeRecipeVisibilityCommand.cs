using MediatR;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Recipes.Commands.ChangeRecipeVisibility;

public class ChangeRecipeVisibilityCommand : IRequest<string>
{
    public Guid Id { get; set; }
    public RecipeVisibility Visibility { get; set; }
}
