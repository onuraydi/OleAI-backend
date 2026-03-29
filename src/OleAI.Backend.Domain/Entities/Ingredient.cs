using OleAI.Backend.Core.Repositories;

namespace OleAI.Backend.Domain.Entities
{
    public class Ingredient : Entity<Guid>
    {
        public string Name { get; private set; }
        public Guid CategoryId { get; private set; }
        public string BaseUnitType { get; private set; } // Maybe take enum.

        public virtual Category Category { get; private set; } = null!;
        public virtual ICollection<IngredientAlias> Aliases { get; private set; }
        public virtual ICollection<UserIngredient> UserIngredients { get; private set; }

        protected Ingredient()
        {
            Name = string.Empty;
            BaseUnitType = string.Empty;
            Aliases = new HashSet<IngredientAlias>();
            UserIngredients = new HashSet<UserIngredient>();
        }

        public Ingredient(Guid id, string name, Guid categoryId, string baseUnitType) : this()
        {
            Id = id;
            Name = name;
            CategoryId = categoryId;
            BaseUnitType = baseUnitType;
        }
    }
}
