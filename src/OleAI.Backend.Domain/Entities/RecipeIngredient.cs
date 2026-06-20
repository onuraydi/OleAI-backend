using OleAI.Backend.Core.Repositories;

namespace OleAI.Backend.Domain.Entities;

public class RecipeIngredient : Entity<Guid>
{
    public Guid RecipeId { get; set; }
    public Guid? IngredientId { get; set; }
    public Guid? IngredientRequestId { get; set; }
    public decimal Amount { get; set; }
    public int UnitId { get; set; }
    public virtual Ingredient? Ingredient { get; set; }
    public virtual IngredientRequest? IngredientRequest { get; set; }

    public RecipeIngredient() { }

    public RecipeIngredient(Guid id, Guid recipeId, Guid? ingredientId, Guid? ingredientRequestId, decimal amount, int unitId) : this()
    {
        Id = id;
        RecipeId = recipeId;
        IngredientId = ingredientId;
        IngredientRequestId = ingredientRequestId;
        Amount = amount;
        UnitId = unitId;
    }
}
