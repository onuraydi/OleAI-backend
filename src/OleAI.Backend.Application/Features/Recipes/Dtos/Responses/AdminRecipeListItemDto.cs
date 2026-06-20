namespace OleAI.Backend.Application.Features.Recipes.Dtos.Responses
{
    public class AdminRecipeListItemDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Visibility { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}
