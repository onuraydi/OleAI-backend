namespace OleAI.Backend.Application.Features.Recipes.Dtos.Responses
{
    public class RecipeIngredientResponse
    {
        public Guid IngredientId { get; set; }
        public string IngredientName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int UnitId { get; set; }
        public int UnitFamily { get; set; } // Map this from Ingredient.UnitFamily
    }
}
