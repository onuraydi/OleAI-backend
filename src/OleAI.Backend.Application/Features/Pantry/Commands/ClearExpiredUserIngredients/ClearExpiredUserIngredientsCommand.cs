using MediatR;

namespace OleAI.Backend.Application.Features.Pantry.Commands.ClearExpiredUserIngredients
{
    public class ClearExpiredUserIngredientsCommand : IRequest<string>
    {
        public Guid userId { get; set; }
    }
}
