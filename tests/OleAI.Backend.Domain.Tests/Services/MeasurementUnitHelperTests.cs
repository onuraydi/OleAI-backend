using FluentAssertions;
using OleAI.Backend.Domain.Enums;
using OleAI.Backend.Domain.Services;
using Xunit;

namespace OleAI.Backend.Domain.Tests.Services;

public class MeasurementUnitHelperTests
{
    [Theory]
    [InlineData(MeasurementCategory.Weight, MeasurementUnit.Gram, true)]
    [InlineData(MeasurementCategory.Weight, MeasurementUnit.Kilogram, true)]
    [InlineData(MeasurementCategory.Weight, MeasurementUnit.Milliliter, false)]
    [InlineData(MeasurementCategory.Volume, MeasurementUnit.Milliliter, true)]
    [InlineData(MeasurementCategory.Count, MeasurementUnit.Piece, true)]
    public void IsCompatible_ShouldReturnExpectedResult(MeasurementCategory category, MeasurementUnit unit, bool expected)
    {
        // Act
        var result = MeasurementUnitHelper.IsCompatible(category, unit);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void GetUnitsForCategory_ShouldReturnCorrectUnits()
    {
        // Act
        var units = MeasurementUnitHelper.GetUnitsForCategory(MeasurementCategory.Weight);

        // Assert
        units.Should().Contain(MeasurementUnit.Gram);
        units.Should().Contain(MeasurementUnit.Kilogram);
        units.Should().NotContain(MeasurementUnit.Milliliter);
    }

    [Theory]
    [InlineData(MeasurementCategory.Weight, MeasurementUnit.Gram)]
    [InlineData(MeasurementCategory.Volume, MeasurementUnit.Liter)]
    [InlineData(MeasurementCategory.Count, MeasurementUnit.Piece)]
    public void GetDefaultUnitForCategory_ShouldReturnCorrectDefault(MeasurementCategory category, MeasurementUnit expected)
    {
        // Act
        var result = MeasurementUnitHelper.GetDefaultUnitForCategory(category);

        // Assert
        result.Should().Be(expected);
    }
}
