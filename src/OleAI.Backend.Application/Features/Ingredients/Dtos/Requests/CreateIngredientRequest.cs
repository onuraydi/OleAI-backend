namespace OleAI.Backend.Application.Features.Ingredients.Dtos.Requests
{
    public class CreateIngredientRequest
    {
        public string Name { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string BaseUnitType { get; set; } = string.Empty;
    }
}
