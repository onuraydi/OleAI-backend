namespace OleAI.Backend.Application.Features.IngredientAliases.Dtos.Responses
{
    public class IngredientAliasResponse
    {
        public Guid Id { get; set; }
        public Guid IngredientId { get; set; }
        public string AliasName { get; set; } = string.Empty;
        public DateTime? DeletedDate { get; set; }
    }
}
