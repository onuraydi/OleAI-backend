using FluentAssertions;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Responses;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Ingredients.Dtos.Responses;

public class IngredientResponseTests
{
    [Fact]
    public void IngredientResponse_ShouldStoreValues()
    {
        var deletedDate = DateTime.UtcNow;
        var dto = new IngredientResponse
        {
            Id = Guid.NewGuid(),
            Name = "Domates",
            CategoryName = "Sebze",
            UnitFamily = MeasurementCategory.Count,
            DeletedDate = deletedDate
        };

        dto.Name.Should().Be("Domates");
        dto.CategoryName.Should().Be("Sebze");
        dto.UnitFamily.Should().Be(MeasurementCategory.Count);
        dto.DeletedDate.Should().Be(deletedDate);
    }
}
