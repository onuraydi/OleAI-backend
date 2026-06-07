namespace OleAI.Backend.Application.Features.Recipes.Dtos.Requests
{
    public class CreateRecipeIngredientRequest
    {
        public Guid? IngredientId { get; set; }
        public string? RequestedName { get; set; }
        public Guid? SuggestedCategoryId { get; set; }
        public decimal Amount { get; set; }
        public int UnitId { get; set; }
    }
}
