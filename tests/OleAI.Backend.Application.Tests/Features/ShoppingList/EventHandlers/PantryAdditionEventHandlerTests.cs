using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.ShoppingList.EventHandlers;
using OleAI.Backend.Application.Features.ShoppingList.Events;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.ShoppingList.EventHandlers;

public class PantryAdditionEventHandlerTests
{
    private readonly Mock<IUserIngredientRepository> _mockUserIngredientRepo;
    private readonly Mock<IIngredientRepository> _mockIngredientRepo;
    private readonly Mock<IIngredientRequestRepository> _mockIngredientRequestRepo;
    private readonly PantryAdditionEventHandler _handler;

    public PantryAdditionEventHandlerTests()
    {
        _mockUserIngredientRepo = new Mock<IUserIngredientRepository>();
        _mockIngredientRepo = new Mock<IIngredientRepository>();
        _mockIngredientRequestRepo = new Mock<IIngredientRequestRepository>();
        _handler = new PantryAdditionEventHandler(
            _mockUserIngredientRepo.Object,
            _mockIngredientRepo.Object,
            _mockIngredientRequestRepo.Object);
    }

    [Fact]
    public async Task Handle_WhenIngredientNotInPantry_ShouldCreateNewEntry()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ingredientId = Guid.NewGuid();

        _mockUserIngredientRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<UserIngredient, bool>>>(),
                null, It.IsAny<bool>(), true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserIngredient?)null);

        var notification = new ItemPurchasedEvent
        {
            UserId = userId,
            IngredientId = ingredientId,
            Quantity = 500,
            Unit = MeasurementUnit.Gram
        };

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert: AddAsync çağrıldı ve doğru değerlerle
        _mockUserIngredientRepo.Verify(
            r => r.AddAsync(
                It.Is<UserIngredient>(ui =>
                    ui.UserId == userId &&
                    ui.IngredientId == ingredientId &&
                    Math.Abs(ui.Amount - 500d) < 0.0001d &&
                    ui.Unit == MeasurementUnit.Gram),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenIngredientAlreadyInPantry_ShouldAddQuantity()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ingredientId = Guid.NewGuid();
        var existing = UserIngredient.CreateStandard(Guid.NewGuid(), userId, ingredientId, 300, MeasurementUnit.Gram);

        _mockUserIngredientRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<UserIngredient, bool>>>(),
                null, It.IsAny<bool>(), true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _mockUserIngredientRepo
            .Setup(r => r.UpdateAsync(existing, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var notification = new ItemPurchasedEvent
        {
            UserId = userId,
            IngredientId = ingredientId,
            Quantity = 200,
            Unit = MeasurementUnit.Gram
        };

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert: miktar 300+200=500 olmalı, AddAsync çağrılmamalı
        existing.Amount.Should().BeApproximately(500d, 0.0001d);
        _mockUserIngredientRepo.Verify(
            r => r.UpdateAsync(existing, It.IsAny<CancellationToken>()),
            Times.Once);
        _mockUserIngredientRepo.Verify(
            r => r.AddAsync(It.IsAny<UserIngredient>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenIngredientExistsButDeleted_ShouldRestoreAndSetQuantity()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ingredientId = Guid.NewGuid();
        var existing = UserIngredient.CreateStandard(Guid.NewGuid(), userId, ingredientId, 300, MeasurementUnit.Gram);
        existing.DeletedDate = DateTime.UtcNow;

        _mockUserIngredientRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<UserIngredient, bool>>>(),
                null, It.IsAny<bool>(), true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _mockUserIngredientRepo
            .Setup(r => r.UpdateAsync(existing, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var notification = new ItemPurchasedEvent
        {
            UserId = userId,
            IngredientId = ingredientId,
            Quantity = 200,
            Unit = MeasurementUnit.Gram
        };

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        existing.DeletedDate.Should().BeNull();
        existing.Amount.Should().BeApproximately(200d, 0.0001d);
        _mockUserIngredientRepo.Verify(
            r => r.UpdateAsync(existing, It.IsAny<CancellationToken>()),
            Times.Once);
        _mockUserIngredientRepo.Verify(
            r => r.AddAsync(It.IsAny<UserIngredient>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
