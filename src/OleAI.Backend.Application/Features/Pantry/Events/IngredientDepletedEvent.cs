using MediatR;

namespace OleAI.Backend.Application.Features.Pantry.Events
{
    public class IngredientDepletedEvent : INotification
    {
        public Guid UserId { get; set; }
        public Guid MasterIngredientId { get; set; }
        public string IngredientName { get; set; } = string.Empty;
    }
}
