namespace OleAI.Backend.Application.Services.AiRecommendation
{
    /// <summary>
    /// AI öneri motorunun henüz hazır olmadığı durumda kullanılan geçici (stub) implementasyon.
    /// Gerçek Python/Scikit-learn servisi hazır olduğunda HttpAiRecommendationService ile değiştirilecektir.
    /// </summary>
    public class DummyAiRecommendationService : IAiRecommendationService
    {
        public Task<AiRecommendationResponse> GetRecommendationsAsync(
            AiRecommendationPayload payload,
            CancellationToken cancellationToken = default)
        {
            // Stub: Boş öneri listesi döner.
            return Task.FromResult(new AiRecommendationResponse());
        }

        public Task SyncRecipeAsync(
            Guid recipeId,
            string title,
            string ingredients,
            string? category = null,
            List<string>? tags = null,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
