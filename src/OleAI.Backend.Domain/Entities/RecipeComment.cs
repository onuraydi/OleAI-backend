using OleAI.Backend.Core.Repositories;
using OleAI.Backend.Domain.Entities;

public class RecipeComment : Entity<Guid>
{
    public Guid RecipeId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedDate { get; set; }

    public virtual User User { get; set; }
    public virtual Recipe Recipe { get; set; }

    public RecipeComment() { }

    public RecipeComment(Guid id, Guid recipeId, Guid userId, string content) : this()
    {
        RecipeId = recipeId;
        UserId = userId;
        Content = content;
        CreatedDate = DateTime.UtcNow;
    }
}
