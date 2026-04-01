namespace OleAI.Backend.Application.Features.Ingredients.Dtos.Responses
{
    public class CreatedIngredientResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
