using FluentAssertions;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Responses;
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
            BaseUnitType = "adet",
            DeletedDate = deletedDate
        };

        dto.Name.Should().Be("Domates");
        dto.CategoryName.Should().Be("Sebze");
        dto.BaseUnitType.Should().Be("adet");
        dto.DeletedDate.Should().Be(deletedDate);
    }
}
