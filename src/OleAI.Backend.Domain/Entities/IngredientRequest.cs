using OleAI.Backend.Core.Repositories;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Domain.Entities
{
    public class IngredientRequest : Entity<Guid>
    {
        public Guid UserId { get; private set; }
        public string RequestedName { get; private set; }
        public IngredientRequestStatus Status { get; private set; }
        public string? LlmAnalysisResult { get; private set; }
        public string? AdminNote { get; private set; }

        public virtual User User { get; private set; } = null!;

        protected IngredientRequest()
        {
            RequestedName = string.Empty;
        }

        public IngredientRequest(Guid id, Guid userId, string requestedName) : this()
        {
            Id = id;
            UserId = userId;
            RequestedName = requestedName;
            Status = IngredientRequestStatus.Pending;
        }

        // Domain Behaviors - iş kuralları

        public void UpdateLlmAnalysis(string analysisResult)
        {
            LlmAnalysisResult = analysisResult;
        }

        public void ResolveAsApproved()
        {
            Status = IngredientRequestStatus.ApprovedAsNew;
        }

        public void ResolveAsMerged(string adminNote)
        {
            Status = IngredientRequestStatus.MergedWithAlias;
            AdminNote = adminNote;
        }

        public void Reject(string reason)
        {
            Status = IngredientRequestStatus.Rejected;
            AdminNote = reason;
        }
    }
}
