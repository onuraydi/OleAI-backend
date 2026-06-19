using MediatR;

namespace OleAI.Backend.Application.Features.ShoppingLists.Commands.SyncDepletedIngredients
{
    public class SyncDepletedIngredientsCommand : IRequest<string>
    {
        public List<DepletedIngredientRestockDto> ItemsToRestock { get; set; } = new();
    }

    public class DepletedIngredientRestockDto
    {
        public Guid IngredientId { get; set; }
        public double Quantity { get; set; }
        public int Unit { get; set; }
    }
}
