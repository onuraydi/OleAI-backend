using OleAI.Backend.Core.Repositories;

namespace OleAI.Backend.Domain.Entities
{
    public class UserIngredient : Entity<Guid>
    {
        public Guid UserId { get; private set; }
        public Guid? IngredientId { get; private set; }
        public Guid? IngredientRequestId { get; private set; }
        public double Quantity { get; private set; }
        public DateTime? ExpirationDate { get; private set; }


        public virtual User User { get; private set; } = null!;
        public virtual Ingredient? Ingredient { get; private set; }
        public virtual IngredientRequest IngredientRequest { get; private set; }

        protected UserIngredient() { }

        // Onay bekleyen ürün ekleme (Sistemde olmayan)
        public static UserIngredient CreateStandard(Guid id, Guid userId, Guid ingredientId, double quantity, DateTime? expirationDate = null)
        {
            return new UserIngredient
            {
                Id = id,
                UserId = userId,
                IngredientId = ingredientId,
                Quantity = quantity,
                ExpirationDate = expirationDate
            };
        }

        // Onay bekleyen ürün ekleme (Sistemde olmayan)
        public static UserIngredient CreatePending(Guid id, Guid userId, Guid ingredientRequestId, double quantity, DateTime? expirationDate = null)
        {
            return new UserIngredient
            {
                Id = id,
                UserId = userId,
                IngredientRequestId = ingredientRequestId,
                Quantity = quantity,
                ExpirationDate = expirationDate
            };
        }

        // Domain Behaviors
        public void Consume(double amountToConsume)
        {
            if (amountToConsume <= 0)
                throw new ArgumentException(DomainConstants.InvalidConsumptionAmountMessage);

            if (Quantity < amountToConsume)
                throw new InvalidOperationException(DomainConstants.InsufficientIngredientMessage);

            Quantity -= amountToConsume;
        }

        public void AddQuantity(double amountToAdd)
        {
            if (amountToAdd <= 0)
                throw new ArgumentException(DomainConstants.InvalidAdditionAmountMessage);

            Quantity += amountToAdd;
        }

        public void ApprovePendingRequest(Guid masterIngredientId)
        {
            if (IngredientRequestId == null)
                throw new InvalidOperationException(DomainConstants.ProductAlreadyApprovedOrRequestIdMissingMessage);

            IngredientId = masterIngredientId;
            // Onaylandığı için request bağlantısını koparabilir veya log için tutabiliriz.
            // Şimdilik dolapta aktif hale gelmesi için Master ID'yi set ediyoruz.
        }
    }
}
