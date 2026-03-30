namespace OleAI.Backend.Application.Features.Pantry.Dtos.Responses
{
    public class PantryItemResponse
    {
        public Guid Id { get; set; }
        public Guid? MasterIngredientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public double Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public bool IsPendingApproval { get; set; } // LLM onayı bekliyor mu?
    }
}
