using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Ingredients.Dtos.Requests
{
    public class UpdateIngredientRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public MeasurementCategory UnitFamily { get; set; } = MeasurementCategory.Other;
        public bool IsPolarizing { get; set; } = false;
        public int? EstimatedShelfLifeInDays { get; set; }
        public ExpirationMode DefaultExpirationMode { get; set; } = ExpirationMode.Auto;
        public bool IsStaple { get; set; } = false;
    }
}
