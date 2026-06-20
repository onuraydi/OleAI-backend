namespace OleAI.Backend.Application.Features.IngredientCategories.Dtos.Requests
{
    public class UpdateCategoryRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string IconName { get; set; } = string.Empty;
    }
}
