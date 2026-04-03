namespace OleAI.Backend.Application.Features.Pantry.Dtos.Responses
{
    public class ResolvedRequestsResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string RequestedName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // Enum'ın string karşılığını döneceğiz
        public string? AdminNote { get; set; }
        public string? LlmAnalysisResult { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
