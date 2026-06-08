using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.ShoppingList.Constants;
using OleAI.Backend.Application.Features.ShoppingList.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.ShoppingList.Rules;

public class ShoppingListBusinessRulesTests
{
    private readonly Mock<IShoppingListItemRepository> _mockShoppingListRepo;
    private readonly Mock<IIngredientRepository> _mockIngredientRepo;
    private readonly ShoppingListBusinessRules _rules;

    public ShoppingListBusinessRulesTests()
    {
        _mockShoppingListRepo = new Mock<IShoppingListItemRepository>();
        _mockIngredientRepo = new Mock<IIngredientRepository>();

        _rules = new ShoppingListBusinessRules(
            _mockShoppingListRepo.Object,
            _mockIngredientRepo.Object);
    }

    // ─── ItemMustNotAlreadyExistInActiveList ─────────────────────────────────

    [Fact]
    public async Task ItemMustNotAlreadyExistInActiveList_WhenItemExists_ShouldThrow()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ingredientId = Guid.NewGuid();

        _mockShoppingListRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<ShoppingListItem, bool>>>(),
                It.IsAny<Func<IQueryable<ShoppingListItem>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ShoppingListItem, object>>?>(),
                false,
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ShoppingListItem.CreateStandard(userId, ingredientId, 1, MeasurementUnit.Piece));

        // Act & Assert
        await FluentActions.Invoking(() =>
                _rules.ItemMustNotAlreadyExistInActiveList(userId, ingredientId))
            .Should().ThrowAsync<BusinessException>()
            .WithMessage(ShoppingListMessages.ProductAlreadyInList);
    }

    [Fact]
    public async Task ItemMustNotAlreadyExistInActiveList_WhenItemNotExists_ShouldNotThrow()
    {
        // Arrange
        _mockShoppingListRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<ShoppingListItem, bool>>>(),
                It.IsAny<Func<IQueryable<ShoppingListItem>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ShoppingListItem, object>>?>(),
                false,
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ShoppingListItem?)null);

        // Act & Assert: exception fırlatmamalı
        await FluentActions.Invoking(() =>
                _rules.ItemMustNotAlreadyExistInActiveList(Guid.NewGuid(), Guid.NewGuid()))
            .Should().NotThrowAsync();
    }

    // ─── GetItemOrThrow ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetItemOrThrow_WhenItemExists_ShouldReturnItem()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var item = ShoppingListItem.CreateStandard(userId, Guid.NewGuid(), 1, MeasurementUnit.Gram);

        _mockShoppingListRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<ShoppingListItem, bool>>>(),
                It.IsAny<Func<IQueryable<ShoppingListItem>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ShoppingListItem, object>>?>(),
                false,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        // Act
        var result = await _rules.GetItemOrThrow(item.Id, userId);

        // Assert
        result.Should().Be(item);
    }

    [Fact]
    public async Task GetItemOrThrow_WhenItemNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _mockShoppingListRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<ShoppingListItem, bool>>>(),
                It.IsAny<Func<IQueryable<ShoppingListItem>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ShoppingListItem, object>>?>(),
                false,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ShoppingListItem?)null);

        // Act & Assert
        await FluentActions.Invoking(() =>
                _rules.GetItemOrThrow(Guid.NewGuid(), Guid.NewGuid()))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage(ShoppingListMessages.ItemNotFoundOrUnauthorized);
    }

    // ─── IngredientMustExist ───────────────────────────────────────────────────

    [Fact]
    public async Task IngredientMustExist_WhenNotFound_ShouldThrowBusinessException()
    {
        // Arrange
        _mockIngredientRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<Ingredient, bool>>>(),
                It.IsAny<Func<IQueryable<Ingredient>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Ingredient, object>>?>(),
                false,
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ingredient?)null);

        // Act & Assert
        await FluentActions.Invoking(() =>
                _rules.IngredientMustExist(Guid.NewGuid()))
            .Should().ThrowAsync<BusinessException>()
            .WithMessage(ShoppingListMessages.IngredientNotFound);
    }

    [Fact]
    public async Task IngredientMustExist_WhenFound_ShouldNotThrow()
    {
        // Arrange
        _mockIngredientRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<Ingredient, bool>>>(),
                It.IsAny<Func<IQueryable<Ingredient>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Ingredient, object>>?>(),
                false,
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Ingredient(Guid.NewGuid(), "Domates", Guid.NewGuid(), MeasurementCategory.Weight));

        // Act & Assert
        await FluentActions.Invoking(() =>
                _rules.IngredientMustExist(Guid.NewGuid()))
            .Should().NotThrowAsync();
    }

    [Fact]
    public void ItemMustNotBeAlreadyPurchased_WhenPurchased_ShouldThrowBusinessException()
    {
        // Arrange
        var item = ShoppingListItem.CreateStandard(Guid.NewGuid(), Guid.NewGuid(), 1, MeasurementUnit.Piece);
        item.MarkAsPurchased();

        // Act
        Action act = () => ShoppingListBusinessRules.ItemMustNotBeAlreadyPurchased(item);

        // Assert
        act.Should().Throw<BusinessException>()
            .WithMessage(ShoppingListMessages.AlreadyPurchased);
    }

    [Fact]
    public void ItemMustNotBeAlreadyPurchased_WhenNotPurchased_ShouldNotThrow()
    {
        // Arrange
        var item = ShoppingListItem.CreateStandard(Guid.NewGuid(), Guid.NewGuid(), 1, MeasurementUnit.Piece);

        // Act
        Action act = () => ShoppingListBusinessRules.ItemMustNotBeAlreadyPurchased(item);

        // Assert
        act.Should().NotThrow();
    }
}

