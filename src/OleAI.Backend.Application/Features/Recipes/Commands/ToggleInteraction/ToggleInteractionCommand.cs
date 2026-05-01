using MediatR;

namespace OleAI.Backend.Application.Features.Recipes.Commands.ToggleInteraction;

public class ToggleInteractionCommand : IRequest<string>
{
    public Guid RecipeId { get; set; }
    /// <summary>
    /// Geçerli Değerler: "Like", "Dislike", "Save" 
    /// </summary>
    public string InteractionType { get; set; } = string.Empty; // Daha daha sonra enum kullanılabilir.
}
