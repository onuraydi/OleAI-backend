using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Pantry.Dtos.Responses
{
    public class IngredientSearchResultResponse
    {
        public Guid MasterIngredientId { get; set; }
        public Guid? AliasId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public bool IsAlias { get; set; }
        public MeasurementCategory UnitFamily { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? CategoryIconName { get; set; }
        public int? EstimatedShelfLifeInDays { get; set; }
        public ExpirationMode DefaultExpirationMode { get; set; }
    }
}
