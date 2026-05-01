using OleAI.Backend.Core.Repositories;

namespace OleAI.Backend.Domain.Entities;

public class RecipeComment : Entity<Guid>
{
    public Guid RecipeId { get; private set; }
    public Guid UserId { get; private set; }
    public string Content { get; private set; }

    public Guid? ParentCommentId { get; private set; }

    public virtual User User { get; private set; }
    public virtual Recipe Recipe { get; private set; }
    public virtual RecipeComment? ParentComment { get; private set; }
    public virtual ICollection<RecipeComment> Replies { get; private set; }

    public RecipeComment()
    {
        Replies = new HashSet<RecipeComment>();
    }

    public RecipeComment(Guid id, Guid recipeId, Guid userId, string content, Guid? parentCommentId = null) : this()
    {
        Id = id;
        RecipeId = recipeId;
        UserId = userId;
        Content = content;
        ParentCommentId = parentCommentId;
        CreatedDate = DateTime.UtcNow;
        Replies = new HashSet<RecipeComment>();
    }

    public void UpdateContent(string newContent)
    {
        Content = newContent;
        UpdatedDate = DateTime.UtcNow;
    }
}
