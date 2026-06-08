using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Pantry.Dtos;

public class IngredientCacheModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public string CategoryIconName { get; set; } = string.Empty;
    public MeasurementCategory UnitFamily { get; set; }
}
