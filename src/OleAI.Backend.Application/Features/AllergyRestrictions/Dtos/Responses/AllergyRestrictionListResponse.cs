namespace OleAI.Backend.Application.Features.AllergyRestrictions.Dtos.Responses;

public class AllergyRestrictionListResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string IconName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int ExcludedIngredientCount { get; set; }
    public int ExcludedCategoryCount { get; set; }
    public DateTime? DeletedDate { get; set; }
}
