using OleAI.Backend.Core.Repositories;

namespace OleAI.Backend.Domain.Entities;

public class RecipeCategoryMapping : Entity<Guid>
{
    public Guid RecipeId { get; private set; }
    public int RecipeCategoryId { get; private set; }

    public virtual Recipe Recipe { get; private set; } = null!;
    public virtual RecipeCategory RecipeCategory { get; private set; } = null!;

    private RecipeCategoryMapping() { }

    public RecipeCategoryMapping(Guid id, Guid recipeId, int recipeCategoryId) : this()
    {
        Id = id;
        RecipeId = recipeId;
        RecipeCategoryId = recipeCategoryId;
    }
}
