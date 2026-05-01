namespace OleAI.Backend.Application.Features.Recipes.Dtos.Responses
{
    public class RecipeIngredientResponse
    {
        public Guid IngredientId { get; set; }
        public decimal Amount { get; set; }
        public int UnitId { get; set; }
    }
}
