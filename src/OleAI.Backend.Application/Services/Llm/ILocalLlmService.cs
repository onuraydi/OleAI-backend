namespace OleAI.Backend.Application.Services.Llm
{
    public interface ILocalLlmService
    {
        /// <summary>
        /// Kullanıcının girdiği serbest metni alır, mevcut veritabanı bağlamıyla LLM'e sorar ve JSON formatında bir analiz döner.
        /// </summary>
        Task<string> AnalyzeIngredientRequestAsync(string requestedName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Bir tarifin başlık ve açıklamasını analiz ederek zararlı/spam içerik olup olmadığını denetler.
        /// Temiz ise True, zararlı/spam ise False döner.
        /// </summary>
        Task<bool> CheckRecipeContentForSpamAsync(string title, string description, CancellationToken cancellationToken = default);

        Task<string> AnalyzeRecipeContentAsync(string description, List<string> steps, CancellationToken cancellationToken = default);
    }
}
