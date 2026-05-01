using MediatR;

namespace OleAI.Backend.Application.Features.Recipes.Commands.DeleteRecipe;

public class DeleteRecipeCommand : IRequest<string>
{
    public Guid Id { get; set; }
}
