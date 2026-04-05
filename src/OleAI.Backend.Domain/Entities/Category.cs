using OleAI.Backend.Core.Repositories;

namespace OleAI.Backend.Domain.Entities
{
    public class Category : Entity<Guid>
    {
        public string Name { get; private set; }
        public virtual ICollection<Ingredient> Ingredients { get; private set; }

        protected Category()
        {
            Name = string.Empty;
            Ingredients = new HashSet<Ingredient>();
        }

        public Category(Guid id, string name) : this()
        {
            Id = id;
            Name = name;
        }
    }
}
