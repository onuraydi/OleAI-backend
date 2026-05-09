using OleAI.Backend.Application.Features.IngredientCategories.Dtos.Response;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Responses;

namespace OleAI.Backend.Application.Features.AllergyRestrictions.Dtos.Responses;

public class AllergyRestrictionDetailResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }

    public List<IngredientResponse> ExcludedIngredients { get; set; } = new();
    public List<CategoryResponse> ExcludedCategories { get; set; } = new();
}
