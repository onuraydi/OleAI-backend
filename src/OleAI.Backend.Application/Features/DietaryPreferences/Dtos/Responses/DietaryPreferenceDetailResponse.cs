using OleAI.Backend.Application.Features.IngredientCategories.Dtos.Response;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Responses;

namespace OleAI.Backend.Application.Features.DietaryPreferences.Dtos.Responses;

public class DietaryPreferenceDetailResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string IconName { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    public List<IngredientResponse> ExcludedIngredients { get; set; } = new();
    public List<CategoryResponse> ExcludedCategories { get; set; } = new();
}
