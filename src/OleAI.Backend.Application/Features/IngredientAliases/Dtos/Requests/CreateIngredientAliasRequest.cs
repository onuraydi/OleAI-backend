namespace OleAI.Backend.Application.Features.IngredientAliases.Dtos.Requests
{
    public class CreateIngredientAliasRequest
    {
        public Guid IngredientId { get; set; }
        public string AliasName { get; set; } = string.Empty;
    }
}
