using OleAI.Backend.Core.Repositories;

namespace OleAI.Backend.Domain.Entities;

public class RecipeInteraction : Entity<Guid>
{
    public Guid RecipeId { get; private set; }
    public Guid UserId { get; private set; }

    public bool IsLiked { get; private set; }
    public bool IsDisliked { get; private set; }
    public bool IsSaved { get; private set; }

    public virtual Recipe Recipe { get; private set; }
    public virtual User User { get; private set; }

    public RecipeInteraction() { }

    public RecipeInteraction(Guid id, Guid recipeId, Guid userId) : this()
    {
        RecipeId = recipeId;
        UserId = userId;
    }


    public void ToggleLike()
    {
        IsLiked = !IsLiked;
        if (IsLiked) IsDisliked = false;
    }

    public void ToggleDislike()
    {
        IsDisliked = !IsDisliked;
        if (IsDisliked) IsLiked = false;
    }

    public void ToggleSave()
    {
        IsSaved = !IsSaved;
    }
}
