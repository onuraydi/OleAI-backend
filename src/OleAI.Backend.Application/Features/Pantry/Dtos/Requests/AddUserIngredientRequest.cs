namespace OleAI.Backend.Application.Features.Pantry.Dtos.Requests
{
    public class AddUserIngredientRequest
    {
        public Guid UserId { get; set; }
        public Guid? IngredientId { get; set; } // Listeden seçildiyse dolu gelir
        public string? RequestedName { get; set; } // Kullanıcı serbest metin girdiyse dolu gelir
        public double Quantity { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}
