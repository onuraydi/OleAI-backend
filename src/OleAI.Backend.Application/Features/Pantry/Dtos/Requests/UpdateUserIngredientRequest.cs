using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Pantry.Dtos.Requests
{
    public class UpdateUserIngredientRequest
    {
        public Guid UserId { get; set; }    // Controller'da token'dan doldurulacak
        public Guid Id { get; set; }        // Güncellenecek UserIngredient kaydının ID'si
        public double NewQuantity { get; set; }
        public MeasurementUnit? NewUnit { get; set; }
    }
}
