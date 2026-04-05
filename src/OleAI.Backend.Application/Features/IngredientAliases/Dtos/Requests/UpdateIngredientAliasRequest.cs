namespace OleAI.Backend.Application.Features.IngredientAliases.Dtos.Requests
{
    public class UpdateIngredientAliasRequest
    {
        public Guid Id { get; set; }
        public Guid IngredientId { get; set; }
        public string AliasName { get; set; } = string.Empty;
    }
}
