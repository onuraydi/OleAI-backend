using OleAI.Backend.Application.Services.Restock;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Services.Restock;

public class SmartRestockServiceTests
{
    private readonly SmartRestockService _service;

    public SmartRestockServiceTests()
    {
        _service = new SmartRestockService();
    }

    [Theory]
    [InlineData(100, 150, MeasurementUnit.Gram)] // Fazlasıyla var
    [InlineData(100, 100, MeasurementUnit.Piece)] // Tam yetiyor
    public void CalculateRestockAmount_WhenPantryHasEnough_ShouldReturnNull(double required, double pantry, MeasurementUnit unit)
    {
        // Act
        var result = _service.CalculateRestockAmount(required, pantry, unit);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData(100, 96, MeasurementUnit.Piece)] // %4 eksik -> %5'ten küçük
    [InlineData(1000, 960, MeasurementUnit.Gram)] // %4 eksik -> %5'ten küçük
    public void CalculateRestockAmount_WhenMissingAmountIsUnderPercentageTolerance_ShouldReturnNull(double required, double pantry, MeasurementUnit unit)
    {
        // Act
        var result = _service.CalculateRestockAmount(required, pantry, unit);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData(100, 92, MeasurementUnit.Gram)] // %8 eksik ama 8 gram eksik (10'dan küçük)
    [InlineData(50, 42, MeasurementUnit.Milliliter)] // %16 eksik ama 8 ml eksik (10'dan küçük)
    public void CalculateRestockAmount_WhenMissingAmountIsUnderAbsoluteTolerance_ShouldReturnNull(double required, double pantry, MeasurementUnit unit)
    {
        // Act
        var result = _service.CalculateRestockAmount(required, pantry, unit);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData(100, 50, MeasurementUnit.Gram, 250)] // 50g eksik -> 250g yuvarla
    [InlineData(400, 100, MeasurementUnit.Gram, 500)] // 300g eksik -> 500g yuvarla
    public void CalculateRestockAmount_ForGram_ShouldRoundUpTo250(double required, double pantry, MeasurementUnit unit, double expected)
    {
        // Act
        var result = _service.CalculateRestockAmount(required, pantry, unit);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(500, 100, MeasurementUnit.Milliliter, 500)] // 400ml eksik -> 500ml yuvarla
    [InlineData(1000, 400, MeasurementUnit.Milliliter, 1000)] // 600ml eksik -> 1000ml yuvarla
    public void CalculateRestockAmount_ForMilliliter_ShouldRoundUpTo500(double required, double pantry, MeasurementUnit unit, double expected)
    {
        // Act
        var result = _service.CalculateRestockAmount(required, pantry, unit);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1.2, 1.0, MeasurementUnit.Kilogram, 0.5)] // 0.2kg eksik -> 0.5kg yuvarla
    [InlineData(2.0, 1.2, MeasurementUnit.Kilogram, 1.0)] // 0.8kg eksik -> 1.0kg yuvarla
    [InlineData(2.5, 1.3, MeasurementUnit.Liter, 1.5)] // 1.2L eksik -> 1.5L yuvarla
    public void CalculateRestockAmount_ForKilogramAndLiter_ShouldRoundUpTo0_5(double required, double pantry, MeasurementUnit unit, double expected)
    {
        // Act
        var result = _service.CalculateRestockAmount(required, pantry, unit);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(2.3, 1.0, MeasurementUnit.Piece, 2)] // 1.3 adet eksik -> 2 adet yuvarla
    [InlineData(5.0, 3.8, MeasurementUnit.Piece, 2)]
    public void CalculateRestockAmount_ForPiece_ShouldRoundUpToNextInteger(double required, double pantry, MeasurementUnit unit, double expected)
    {
        // Act
        var result = _service.CalculateRestockAmount(required, pantry, unit);

        // Assert
        Assert.Equal(expected, result);
    }
}
