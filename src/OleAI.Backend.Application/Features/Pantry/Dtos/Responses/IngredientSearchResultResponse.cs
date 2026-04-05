namespace OleAI.Backend.Application.Features.Pantry.Dtos.Responses
{
    public class IngredientSearchResultResponse
    {
        public Guid MasterIngredientId { get; set; }
        public string DisplayName { get; set; } = string.Empty; // Bulunan metin (Alias veya Master ad)
        public bool IsAlias { get; set; }
        public string BaseUnitType { get; set; } = string.Empty;
    }
}
