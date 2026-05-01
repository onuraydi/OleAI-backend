namespace OleAI.Backend.Domain.Enums;

public enum RecipeStatus
{
    PendingLlmApproval = 1,
    PendingAdminApproval = 2,
    Published = 3,
    Rejected = 4
}
