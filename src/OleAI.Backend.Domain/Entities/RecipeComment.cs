using OleAI.Backend.Core.Repositories;

namespace OleAI.Backend.Domain.Entities;

public class RecipeComment : Entity<Guid>
{
    public Guid RecipeId { get; private set; }
    public Guid UserId { get; private set; }
    public string Content { get; private set; }

    public Guid? ParentCommentId { get; private set; }
    public int? Rating { get; private set; }

    public virtual User User { get; private set; }
    public virtual Recipe Recipe { get; private set; }
    public virtual RecipeComment? ParentComment { get; private set; }
    public virtual ICollection<RecipeComment> Replies { get; private set; }

    public RecipeComment()
    {
        Replies = new HashSet<RecipeComment>();
    }

    public RecipeComment(Guid id, Guid recipeId, Guid userId, string content, Guid? parentCommentId = null, int? rating = null) : this()
    {
        if (parentCommentId == null)
        {
            if (rating == null || rating < 1 || rating > 5)
                throw new ArgumentException("Ana yorumlar için 1 ile 5 arasında bir puan verilmelidir.");
        }
        else
        {
            if (rating != null)
                throw new ArgumentException("Yanıt niteliğindeki yorumlara puan verilemez.");
        }

        Id = id;
        RecipeId = recipeId;
        UserId = userId;
        Content = content;
        ParentCommentId = parentCommentId;
        Rating = rating;
        CreatedDate = DateTime.UtcNow;
        Replies = new HashSet<RecipeComment>();
    }

    public void Update(string newContent, int? newRating)
    {
        if (ParentCommentId == null && (newRating == null || newRating < 1 || newRating > 5))
            throw new ArgumentException("Ana yorumlar için 1 ile 5 arasında bir puan verilmelidir.");
        if (ParentCommentId != null && newRating != null)
            throw new ArgumentException("Yanıt niteliğindeki yorumlara puan verilemez.");

        Content = newContent;
        if (ParentCommentId == null)
            Rating = newRating;

        UpdatedDate = DateTime.UtcNow;
    }
}
