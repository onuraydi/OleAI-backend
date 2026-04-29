namespace OleAI.Backend.Application.Features.Recipes.Dtos.Responses
{
    public class RecipeDetailResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public List<RecipeIngredientResponse> Ingredients { get; set; } = new();
        public List<RecipeStepResponse> Steps { get; set; } = new();
        public List<RecipeImageResponse> Images { get; set; } = new();
    }
}
