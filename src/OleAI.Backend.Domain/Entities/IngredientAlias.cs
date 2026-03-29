using OleAI.Backend.Core.Repositories;

namespace OleAI.Backend.Domain.Entities
{
    public class IngredientAlias : Entity<Guid>
    {
        public Guid IngredientId { get; private set; }
        public string AliasName { get; private set; }
        public virtual Ingredient Ingredient { get; private set; } = null!;

        protected IngredientAlias()
        {
            AliasName = string.Empty;
        }

        public IngredientAlias(Guid id, Guid ingredientId, string aliasName) : this()
        {
            Id = id;
            IngredientId = ingredientId;
            AliasName = aliasName;
        }
    }
}
