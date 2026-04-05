namespace OleAI.Backend.Application.Features.Pantry.Dtos.Responses
{
    public class AddedUserIngredientResponse
    {
        public Guid Id { get; set; }
        public bool IsPendingApproval { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
