using MediatR;

namespace OleAI.Backend.Application.Features.Users.Commands.UpdateUserDislikedIngredients;

public class UpdateUserDislikedIngredientsCommand : IRequest<string>
{
    public List<Guid> IngredientIds { get; set; } = new();
}
