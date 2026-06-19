using OleAI.Backend.Core.Repositories;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Domain.Entities
{
    public class UserIngredient : Entity<Guid>
    {
        private const double DepletionTolerance = 0.000001d;

        public Guid UserId { get; private set; }
        public Guid? IngredientId { get; private set; }
        public Guid? IngredientAliasId { get; private set; }
        public Guid? IngredientRequestId { get; private set; }

        public double Amount { get; private set; }

        /// <summary>
        /// Kullanıcının bu malzemeyi hangi birimde takip ettiğini belirtir.
        /// Ingredient.UnitFamily ile uyumlu bir birim olmalıdır.
        /// </summary>
        public MeasurementUnit Unit { get; private set; }

        public DateTime? ExpirationDate { get; private set; }

        public virtual User User { get; private set; } = null!;
        public virtual Ingredient? Ingredient { get; private set; }
        public virtual IngredientAlias? IngredientAlias { get; private set; }
        public virtual IngredientRequest? IngredientRequest { get; private set; }

        protected UserIngredient() { }

        public bool IsDepleted => Amount <= DepletionTolerance;

        /// <summary>
        /// Sistemde var olan (onaylı) bir malzeme için pantry girişi oluşturur.
        /// </summary>
        public static UserIngredient CreateStandard(
            Guid id,
            Guid userId,
            Guid ingredientId,
            double amount,
            MeasurementUnit unit,
            DateTime? expirationDate = null,
            Guid? ingredientAliasId = null)
        {
            return new UserIngredient
            {
                Id = id,
                UserId = userId,
                IngredientId = ingredientId,
                IngredientAliasId = ingredientAliasId,
                Amount = amount,
                Unit = unit,
                ExpirationDate = expirationDate
            };
        }

        /// <summary>
        /// Sistemde henüz tanımlı olmayan (onay bekleyen) bir malzeme için pantry girişi oluşturur.
        /// </summary>
        public static UserIngredient CreatePending(
            Guid id,
            Guid userId,
            Guid ingredientRequestId,
            double amount,
            MeasurementUnit unit,
            DateTime? expirationDate = null)
        {
            return new UserIngredient
            {
                Id = id,
                UserId = userId,
                IngredientRequestId = ingredientRequestId,
                Amount = amount,
                Unit = unit,
                ExpirationDate = expirationDate
            };
        }

        // ── Domain Behaviors ─────────────────────────────────────────────────────

        public void Consume(double amountToConsume)
        {
            if (amountToConsume <= 0)
                throw new ArgumentException(DomainConstants.InvalidConsumptionAmountMessage);

            if (amountToConsume > Amount)
                throw new InvalidOperationException(DomainConstants.InsufficientIngredientMessage);

            Amount -= amountToConsume;
        }

        public void AddQuantity(double amountToAdd)
        {
            if (amountToAdd <= 0)
                throw new ArgumentException(DomainConstants.InvalidAdditionAmountMessage);

            Amount += amountToAdd;
        }

        public void UpdateQuantity(double newQuantity, MeasurementUnit newUnit)
        {
            if (newQuantity < 0)
                throw new ArgumentException(DomainConstants.AmountCannotBeNegativeMessage);

            Amount = newQuantity;
            Unit = newUnit;
        }

        public void UpdateExpirationDate(DateTime? expirationDate)
        {
            ExpirationDate = expirationDate;
        }

        public void ApprovePendingRequest(Guid masterIngredientId)
        {
            if (IngredientRequestId == null)
                throw new InvalidOperationException(DomainConstants.ProductAlreadyApprovedOrRequestIdMissingMessage);

            IngredientId = masterIngredientId;
            // IngredientRequestId bağlantısı audit trail için tutulmaktadır.
        }

        /// <summary>
        /// Kullanılan miktar kadar stoku düşer. Miktar sıfırın altına inemez.
        /// </summary>
        public void DecreaseAmount(double usedAmount)
        {
            Amount -= usedAmount;
            if (Amount <= DepletionTolerance)
            {
                Amount = 0;
            }
        }
    }
}
