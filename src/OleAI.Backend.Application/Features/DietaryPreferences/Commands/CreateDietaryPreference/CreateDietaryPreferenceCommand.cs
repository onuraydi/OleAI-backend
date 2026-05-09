using MediatR;

namespace OleAI.Backend.Application.Features.DietaryPreferences.Commands.CreateDietaryPreference;

public class CreateDietaryPreferenceCommand : IRequest<string>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<Guid> ExcludedIngredientIds { get; set; } = new();
    public List<Guid> ExcludedCategoryIds { get; set; } = new();
}
