using OleAI.Backend.Core.Repositories;

namespace OleAI.Backend.Domain.Entities;

public class Tag : Entity<int>
{
    public string Name { get; private set; }

    public virtual ICollection<RecipeTag> RecipeTags { get; private set; }

    private Tag()
    {
        Name = string.Empty;
        RecipeTags = new HashSet<RecipeTag>();
    }

    public Tag(int id, string name) : this()
    {
        Id = id;
        Name = name;
    }
}
