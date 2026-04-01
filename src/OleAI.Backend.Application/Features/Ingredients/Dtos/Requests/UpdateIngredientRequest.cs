namespace OleAI.Backend.Application.Features.Ingredients.Dtos.Requests
{
    public class UpdateIngredientRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string BaseUnitType { get; set; } = string.Empty;
    }
}
