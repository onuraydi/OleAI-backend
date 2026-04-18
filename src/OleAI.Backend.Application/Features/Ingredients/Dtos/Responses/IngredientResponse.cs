using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Ingredients.Dtos.Responses
{
    public class IngredientResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public MeasurementCategory UnitFamily { get; set; }
        public DateTime? DeletedDate { get; set; } // Arayüzde "Pasif" yazabilmek için
    }
}
