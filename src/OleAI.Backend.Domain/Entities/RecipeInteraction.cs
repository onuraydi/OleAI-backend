using OleAI.Backend.Core.Repositories;

public class RecipeInteraction : Entity<Guid>
{
    public Guid RecipeId { get; set; }
    public Guid UserId { get; set; }
    public bool IsLiked { get; set; }
    public bool isDisliked { get; set; }
    public bool IsSaved { get; set; }

    public RecipeInteraction() { }
}
