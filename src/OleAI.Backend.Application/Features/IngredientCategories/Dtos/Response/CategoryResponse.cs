namespace OleAI.Backend.Application.Features.IngredientCategories.Dtos.Response
{
    public class CategoryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string IconName { get; set; } = string.Empty;
        public DateTime? DeletedDate { get; set; }
    }
}
