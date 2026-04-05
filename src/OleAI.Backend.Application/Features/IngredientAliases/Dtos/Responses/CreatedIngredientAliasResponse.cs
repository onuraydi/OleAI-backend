namespace OleAI.Backend.Application.Features.IngredientAliases.Dtos.Responses
{
    public class CreatedIngredientAliasResponse
    {
        public Guid IngredientId { get; set; }
        public string AliasName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
