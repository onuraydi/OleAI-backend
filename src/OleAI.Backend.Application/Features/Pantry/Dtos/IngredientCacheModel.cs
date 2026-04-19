using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Pantry.Dtos;

public class IngredientCacheModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public MeasurementCategory UnitFamily { get; set; }
}
