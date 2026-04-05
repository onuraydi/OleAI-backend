namespace OleAI.Backend.Application.Services.Llm
{
    public interface ILocalLlmService
    {
        /// <summary>
        /// Kullanıcının girdiği serbest metni alır, mevcut veritabanı bağlamıyla LLM'e sorar ve JSON formatında bir analiz döner.
        /// </summary>
        Task<string> AnalyzeIngredientRequestAsync(string requestedName, CancellationToken cancellationToken = default);
    }
}
