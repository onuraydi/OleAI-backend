namespace OleAI.Backend.Application.Features.Pantry.Dtos.Requests
{
    public class PendingRequestDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string RequestedName { get; set; } = string.Empty;
        public int? EstimatedShelfLifeInDays { get; set; }
        public string? LlmAnalysisResult { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
