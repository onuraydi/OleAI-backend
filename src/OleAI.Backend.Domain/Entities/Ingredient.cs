using OleAI.Backend.Core.Repositories;
using OleAI.Backend.Domain.Enums;
using OleAI.Backend.Domain.Services;

namespace OleAI.Backend.Domain.Entities
{
    public class Ingredient : Entity<Guid>
    {
        public string Name { get; private set; }
        public Guid CategoryId { get; private set; }

        /// <summary>
        /// Malzemenin ait olduğu ölçü birimi ailesi.
        /// Örn: Weight → (Gram, Kilogram), Volume → (Milliliter, Liter), Count → (Piece, Bunch, ...)
        /// Kullanıcıların bu aile içindeki herhangi bir birimde ölçüm yapmasına izin verilir.
        /// </summary>
        public MeasurementCategory UnitFamily { get; private set; }

        public virtual Category Category { get; private set; } = null!;
        public virtual ICollection<IngredientAlias> Aliases { get; private set; }
        public virtual ICollection<UserIngredient> UserIngredients { get; private set; }
        public virtual ICollection<ShoppingListItem> ShoppingListItems { get; private set; }
        public virtual ICollection<UserDislikedIngredient> UserDislikedIngredients { get; private set; }

        protected Ingredient()
        {
            Name = string.Empty;
            UnitFamily = MeasurementCategory.Other;
            Aliases = new HashSet<IngredientAlias>();
            UserIngredients = new HashSet<UserIngredient>();
            ShoppingListItems = new HashSet<ShoppingListItem>();
            UserDislikedIngredients = new HashSet<UserDislikedIngredient>();
        }

        public Ingredient(Guid id, string name, Guid categoryId, MeasurementCategory unitFamily = MeasurementCategory.Other)
            : this()
        {
            Id = id;
            Name = name;
            CategoryId = categoryId;
            UnitFamily = unitFamily;
        }

        /// <summary>
        /// Malzemenin ölçü birimi ailesini günceller.
        /// </summary>
        public void SetUnitFamily(MeasurementCategory unitFamily)
        {
            UnitFamily = unitFamily;
        }

        /// <summary>
        /// Verilen birimin bu malzeme için geçerli olup olmadığını kontrol eder.
        /// </summary>
        public bool IsUnitCompatible(MeasurementUnit unit)
            => MeasurementUnitHelper.IsCompatible(UnitFamily, unit);
    }
}
