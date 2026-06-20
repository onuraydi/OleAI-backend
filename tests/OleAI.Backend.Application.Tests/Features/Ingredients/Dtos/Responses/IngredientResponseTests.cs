using FluentAssertions;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Responses;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Ingredients.Dtos.Responses;

public class IngredientResponseTests
{
    [Fact]
    public void Properties_ShouldBeGettableAndSettable()
    {
        var dto = new IngredientResponse();
        var id = System.Guid.NewGuid();

        dto.Id = id;
        dto.Name = "Tomato";
        dto.CategoryName = "Vegetable";
        dto.CategoryIconName = "icon";
        dto.UnitFamily = MeasurementCategory.Weight;
        dto.IsPolarizing = false;
        dto.EstimatedShelfLifeInDays = 7;
        dto.DefaultExpirationMode = ExpirationMode.Auto;
        dto.IsStaple = true;
        dto.DeletedDate = System.DateTime.UtcNow;

        dto.Id.Should().Be(id);
        dto.Name.Should().Be("Tomato");
        dto.CategoryName.Should().Be("Vegetable");
        dto.CategoryIconName.Should().Be("icon");
        dto.UnitFamily.Should().Be(MeasurementCategory.Weight);
        dto.IsPolarizing.Should().BeFalse();
        dto.EstimatedShelfLifeInDays.Should().Be(7);
        dto.DefaultExpirationMode.Should().Be(ExpirationMode.Auto);
        dto.IsStaple.Should().BeTrue();
        dto.DeletedDate.Should().NotBeNull();
    }
}
