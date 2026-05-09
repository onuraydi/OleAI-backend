using OleAI.Backend.Core.Repositories;

namespace OleAI.Backend.Domain.Entities;

public class UserDislikedIngredient : Entity<Guid>
{
    public Guid UserId { get; set; }
    public Guid IngredientId { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Ingredient Ingredient { get; set; } = null!;
}
