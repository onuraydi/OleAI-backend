using OleAI.Backend.Core.Repositories;

namespace OleAI.Backend.Domain.Entities;

public class RecipeIngredient : Entity<Guid>
{
    public Guid RecipeId { get; set; }
    public Guid IngredientId { get; set; }
    public decimal Amount { get; set; }
    public int UnitId { get; set; }
    public virtual Ingredient Ingredient { get; set; } = null!;

    public RecipeIngredient() { }

    public RecipeIngredient(Guid id, Guid recipeId, Guid ingredientId, decimal amount, int unitId) : this()
    {
        Id = id;
        RecipeId = recipeId;
        IngredientId = ingredientId;
        Amount = amount;
        UnitId = unitId;
    }
}
