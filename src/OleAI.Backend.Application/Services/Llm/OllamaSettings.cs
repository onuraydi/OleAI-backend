namespace OleAI.Backend.Application.Services.Llm
{
    public class OllamaSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public string SystemPrompt { get; set; } = string.Empty;
    }
}
