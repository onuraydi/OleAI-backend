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
    }
}
