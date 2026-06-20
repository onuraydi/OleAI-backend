using MediatR;

namespace OleAI.Backend.Application.Features.DietaryPreferences.Commands.UpdateDietaryPreference;

public class UpdateDietaryPreferenceCommand : IRequest<string>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string IconName { get; set; } = "Leaf";
    public bool IsActive { get; set; }
    public List<Guid> ExcludedIngredientIds { get; set; } = new();
    public List<Guid> ExcludedCategoryIds { get; set; } = new();
}
