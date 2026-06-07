using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Pantry.Dtos.Responses
{
    public class IngredientSearchResultResponse
    {
        public Guid MasterIngredientId { get; set; }
        public string DisplayName { get; set; } = string.Empty; // Bulunan metin (Alias veya Master ad)
        public bool IsAlias { get; set; }
        public MeasurementCategory UnitFamily { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
