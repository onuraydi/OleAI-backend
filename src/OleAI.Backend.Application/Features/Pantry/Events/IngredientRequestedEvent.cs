using MediatR;

namespace OleAI.Backend.Application.Features.Pantry.Events
{
    public class IngredientRequestedEvent : INotification
    {
        public Guid RequestId { get; set; }
        public string RequestedName { get; set; } = string.Empty;
    }
}
