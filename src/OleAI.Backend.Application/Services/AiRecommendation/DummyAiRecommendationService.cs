using OleAI.Backend.Application.Services.AiRecommendation;

namespace OleAI.Backend.Application.Services.AiRecommendation
{
    public class DummyAiRecommendationService : IAiRecommendationService
    {
        public Task<AiRecommendationResponse> GetRecommendationsAsync(
            AiRecommendationPayload payload,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new AiRecommendationResponse());
        }

        public Task SyncRecipeAsync(
            Guid recipeId,
            string title,
            List<RecipeIngredientAiDto> ingredients,
            string? category = null,
            List<string>? tags = null,
            bool isPublished = true,
            int popularity = 0,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
