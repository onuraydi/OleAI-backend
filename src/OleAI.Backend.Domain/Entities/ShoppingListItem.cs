using OleAI.Backend.Core.Repositories;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Domain.Entities
{
    public class ShoppingListItem : Entity<Guid>
    {
        public Guid UserId { get; set; }
        public Guid IngredientId { get; set; }

        public decimal Amount { get; set; }
        public MeasuremenetUnit Unit { get; set; }
        public bool IsPurchased { get; set; } = false;

        // Navigation prop.
        public virtual User User { get; set; } = null!;
        public virtual Ingredient Ingredient { get; set; } = null!;

        public ShoppingListItem()
        {
        }

        public ShoppingListItem(Guid userId, Guid ingredientId, decimal amount, MeasuremenetUnit unit)
        {
            UserId = userId;
            IngredientId = ingredientId;
            Amount = amount;
            Unit = unit;
            IsPurchased = false;
        }
    }
}
