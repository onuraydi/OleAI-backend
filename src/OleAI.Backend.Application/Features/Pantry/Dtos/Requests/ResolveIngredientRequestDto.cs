using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Pantry.Dtos.Requests
{
    public class ResolveIngredientRequestDto
    {
        public Guid RequestId { get; set; }
        public ResolutionAction Action { get; set; }

        // Eğer yeni olarak onaylanıyorsa girilmesi gereken alanlar (Admin LLM'in önerisini düzeltip girebilir)
        public Guid? NewCategoryId { get; set; }
        public MeasurementCategory? NewUnitFamily { get; set; }

        // Eğer alias olarak birleştiriliyorsa, var olan Master Ürünün ID'si
        public Guid? MasterIngredientId { get; set; }

        public string? AdminNote { get; set; }
    }
}
