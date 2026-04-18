using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Core.Repositories;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Domain.Entities
{
    public class ShoppingListItem : Entity<Guid>
    {
        public Guid UserId { get; private set; }
        public Guid IngredientId { get; private set; }

        public double Amount { get; private set; }
        public MeasurementUnit Unit { get; private set; }

        public bool IsPurchased { get; private set; }

        // Navigation Properties
        public virtual User User { get; private set; } = null!;
        public virtual Ingredient Ingredient { get; private set; } = null!;

        // ORM (Entity Framework) için boş constructor
        public ShoppingListItem()
        {
        }

        // Yeni kayıt oluşturulurken kullanılacak Constructor
        public ShoppingListItem(Guid userId, Guid ingredientId, double amount, MeasurementUnit unit)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            IngredientId = ingredientId;
            SetAmountAndUnit(amount, unit);
            IsPurchased = false;
        }

        // Davranış 1: Ürünün alındığını işaretleme
        public void MarkAsPurchased()
        {
            if (IsPurchased)
                throw new BusinessException(DomainConstants.ProductAlreadyPurchasedMessage);

            IsPurchased = true;
        }

        // Davranış 2: Miktar ve birim güncelleme
        public void UpdateQuantity(double newAmount, MeasurementUnit newUnit)
        {
            if (IsPurchased)
                throw new BusinessException(DomainConstants.CannotChangePurchasedProductQuantity);

            SetAmountAndUnit(newAmount, newUnit);
        }

        // Kapsülleme (Encapsulation): Miktar validasyonu
        private void SetAmountAndUnit(double amount, MeasurementUnit unit)
        {
            if (amount <= 0)
                throw new BusinessException(DomainConstants.InvalidProductQuantityInList);

            Amount = amount;
            Unit = unit;
        }
    }
}
