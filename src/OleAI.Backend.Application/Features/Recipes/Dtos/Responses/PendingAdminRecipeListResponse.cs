namespace OleAI.Backend.Application.Features.Recipes.Dtos.Responses
{
    public class PendingAdminRecipeListResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}
