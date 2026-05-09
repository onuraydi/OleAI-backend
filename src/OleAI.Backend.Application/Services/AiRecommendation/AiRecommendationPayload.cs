namespace OleAI.Backend.Application.Services.AiRecommendation
{
    /// <summary>
    /// Python/Scikit-learn tabanlı AI servisinin beklediği JSON contract (sözleşme).
    /// Kullanıcının diyet tercihleri, alerjileri ve dışlanan malzeme/kategorileri içerir.
    /// </summary>
    public class AiRecommendationPayload
    {
        public Guid UserId { get; set; }
        public List<string> DietaryPreferences { get; set; } = new();
        public List<string> Allergies { get; set; } = new();
        public List<string> ExcludedIngredients { get; set; } = new();
        public List<string> ExcludedCategories { get; set; } = new();
    }
}
