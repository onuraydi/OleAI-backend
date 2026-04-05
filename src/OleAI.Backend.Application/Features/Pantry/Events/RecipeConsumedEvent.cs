using MediatR;

namespace OleAI.Backend.Application.Features.Pantry.Events
{
    public class RecipeConsumedEvent : INotification
    {
        public Guid UserId { get; set; }
        public Guid RecipeId { get; set; }
        public string RecipeName { get; set; } = string.Empty;

        // key: MasterIngredientId, Value: quantity
        public Dictionary<Guid, double> ConsumedIngredients { get; set; } = new();
    }
}
