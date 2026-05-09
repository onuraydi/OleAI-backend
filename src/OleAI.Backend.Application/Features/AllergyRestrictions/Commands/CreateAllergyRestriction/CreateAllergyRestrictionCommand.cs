using MediatR;

namespace OleAI.Backend.Application.Features.AllergyRestrictions.Commands.CreateAllergyRestriction;

public class CreateAllergyRestrictionCommand : IRequest<string>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<Guid> ExcludedIngredientIds { get; set; } = new();
    public List<Guid> ExcludedCategoryIds { get; set; } = new();
}
