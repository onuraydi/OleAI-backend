using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Core.Repositories;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Domain.Entities
{
    public class ShoppingListItem : Entity<Guid>
    {
        public Guid UserId { get; private set; }
        public Guid? IngredientId { get; private set; }
        public Guid? IngredientRequestId { get; private set; }

        public double Quantity { get; private set; }
        public MeasurementUnit Unit { get; private set; }

        public bool IsPurchased { get; private set; }

        // Navigation Properties
        public virtual User User { get; private set; } = null!;
        public virtual Ingredient? Ingredient { get; private set; }
        public virtual IngredientRequest? IngredientRequest { get; private set; }

        // EF Core için gerekli; encapsulation'ı korumak adına protected
        protected ShoppingListItem() { }

        public static ShoppingListItem CreateStandard(Guid userId, Guid ingredientId, double quantity, MeasurementUnit unit)
        {
            var item = new ShoppingListItem
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                IngredientId = ingredientId,
                IsPurchased = false
            };
            item.SetQuantityAndUnit(quantity, unit);
            return item;
        }

        public static ShoppingListItem CreatePending(Guid userId, IngredientRequest ingredientRequest, double quantity, MeasurementUnit unit)
        {
            var item = new ShoppingListItem
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                IngredientRequestId = ingredientRequest.Id,
                IngredientRequest = ingredientRequest,
                IsPurchased = false
            };
            item.SetQuantityAndUnit(quantity, unit);
            return item;
        }

        public void MarkAsPurchased()
        {
            if (IsPurchased)
                throw new BusinessException(DomainConstants.ProductAlreadyPurchasedMessage);

            IsPurchased = true;
        }

        public void UpdateQuantity(double newQuantity, MeasurementUnit newUnit)
        {
            if (IsPurchased)
                throw new BusinessException(DomainConstants.CannotChangePurchasedProductQuantity);

            SetQuantityAndUnit(newQuantity, newUnit);
        }

        private void SetQuantityAndUnit(double quantity, MeasurementUnit unit)
        {
            if (quantity <= 0)
                throw new BusinessException(DomainConstants.InvalidProductQuantityInList);

            Quantity = quantity;
            Unit = unit;
        }
    }
}
