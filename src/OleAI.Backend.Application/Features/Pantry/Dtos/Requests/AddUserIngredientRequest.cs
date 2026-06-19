using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Pantry.Dtos.Requests
{
    public class AddUserIngredientRequest
    {
        public Guid UserId { get; set; }
        public Guid? IngredientId { get; set; }     // Listeden seçildiyse dolu gelir
        public Guid? AliasId { get; set; }
        public string? RequestedName { get; set; }  // Kullanıcı serbest metin girdiyse dolu gelir
        public Guid? SuggestedCategoryId { get; set; } // Kullanıcı serbest metin girdiğinde önerdiği kategori
        public double Quantity { get; set; }
        public MeasurementUnit Unit { get; set; } = MeasurementUnit.Piece;
        public DateTime? ExpirationDate { get; set; }
        public int? EstimatedShelfLifeInDays { get; set; }
    }
}
