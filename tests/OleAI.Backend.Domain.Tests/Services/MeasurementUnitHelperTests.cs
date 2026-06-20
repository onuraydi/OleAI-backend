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
    [Fact]
    public void IsCompatible_WithInvalidCategory_ShouldUseOtherCategory()
    {
        // Act
        var result = MeasurementUnitHelper.IsCompatible((MeasurementCategory)999, MeasurementUnit.Piece);

        // Assert
        result.Should().BeTrue(); // Other category accepts all units
    }

    [Fact]
    public void GetUnitsForCategory_WithInvalidCategory_ShouldReturnAllUnits()
    {
        // Act
        var units = MeasurementUnitHelper.GetUnitsForCategory((MeasurementCategory)999);

        // Assert
        units.Should().Contain(MeasurementUnit.Piece);
        units.Should().Contain(MeasurementUnit.Gram);
        units.Should().Contain(MeasurementUnit.Liter);
    }

    [Theory]
    [InlineData((MeasurementCategory)999, MeasurementUnit.Piece)]
    public void GetDefaultUnitForCategory_WithInvalidCategory_ShouldReturnPiece(MeasurementCategory category, MeasurementUnit expected)
    {
        // Act
        var result = MeasurementUnitHelper.GetDefaultUnitForCategory(category);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(10, MeasurementUnit.Kilogram, MeasurementUnit.Gram, 10000)]
    [InlineData(5000, MeasurementUnit.Gram, MeasurementUnit.Kilogram, 5)]
    [InlineData(2, MeasurementUnit.Liter, MeasurementUnit.Milliliter, 2000)]
    [InlineData(1500, MeasurementUnit.Milliliter, MeasurementUnit.Liter, 1.5)]
    [InlineData(10, MeasurementUnit.Piece, MeasurementUnit.Piece, 10)]
    [InlineData(5, MeasurementUnit.Piece, MeasurementUnit.Gram, 5)] // Cannot convert piece to gram, should return amount
    public void ConvertAmount_ShouldPerformCorrectConversion(double amount, MeasurementUnit fromUnit, MeasurementUnit toUnit, double expected)
    {
        // Act
        var result = MeasurementUnitHelper.ConvertAmount(amount, fromUnit, toUnit);

        // Assert
        result.Should().Be(expected);
    }
}
