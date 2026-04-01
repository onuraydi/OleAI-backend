namespace OleAI.Backend.Application.Features.IngredientCategories.Dtos.Response
{
    public class CreatedCategoryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
