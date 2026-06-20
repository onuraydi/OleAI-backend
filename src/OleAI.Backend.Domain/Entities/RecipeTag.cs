using OleAI.Backend.Core.Repositories;

namespace OleAI.Backend.Domain.Entities;

public class RecipeTag : Entity<Guid>
{
    public Guid RecipeId { get; private set; }
    public int TagId { get; private set; }

    public virtual Recipe Recipe { get; private set; } = null!;
    public virtual Tag Tag { get; private set; } = null!;

    private RecipeTag() { }

    public RecipeTag(Guid id, Guid recipeId, int tagId) : this()
    {
        Id = id;
        RecipeId = recipeId;
        TagId = tagId;
    }
}
