namespace OleAI.Backend.Application.Features.Recipes.Dtos.Responses
{
    public class RecipeImageResponse
    {
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; }
        public Guid? StepId { get; set; }
    }
}
