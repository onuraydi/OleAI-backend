using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Pantry.EventHandlers;
using OleAI.Backend.Application.Features.Pantry.Events;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Pantry.EventHandlers;

public class ShoppingListEventHandlerTests
{
    private readonly Mock<IShoppingListItemRepository> _mockShoppingListRepo;
    private readonly Mock<IIngredientRepository> _mockIngredientRepo;
    private readonly Mock<ICacheService> _mockCache;
    private readonly ShoppingListEventHandler _handler;

    public ShoppingListEventHandlerTests()
    {
        _mockShoppingListRepo = new Mock<IShoppingListItemRepository>();
        _mockIngredientRepo = new Mock<IIngredientRepository>();
        _mockCache = new Mock<ICacheService>();

        _handler = new ShoppingListEventHandler(
            _mockShoppingListRepo.Object,
            _mockIngredientRepo.Object,
            _mockCache.Object);
    }

    [Fact]
    public async Task Handle_WhenItemAlreadyInActiveList_ShouldNotAddDuplicate()
    {
        // Arrange: aynı malzeme zaten listede (IsPurchased=false)
        var userId = Guid.NewGuid();
        var ingredientId = Guid.NewGuid();

        _mockShoppingListRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<ShoppingListItem, bool>>>(),
                It.IsAny<Func<IQueryable<ShoppingListItem>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ShoppingListItem, object>>?>(),
                false,
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ShoppingListItem(userId, ingredientId, 1, MeasurementUnit.Piece));

        var notification = new IngredientDepletedEvent
        {
            UserId = userId,
            MasterIngredientId = ingredientId,
            IngredientName = "Süt"
        };

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert: AddAsync hiç çağrılmamalı
        _mockShoppingListRepo.Verify(
            r => r.AddAsync(It.IsAny<ShoppingListItem>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenIngredientInRedis_ShouldNotHitDatabase()
    {
        // Arrange: Item listede yok, malzeme Redis'te mevcut
        var userId = Guid.NewGuid();
        var ingredientId = Guid.NewGuid();
        var ingredient = new Ingredient(ingredientId, "Süt", Guid.NewGuid(), MeasurementCategory.Volume);

        _mockShoppingListRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<ShoppingListItem, bool>>>(),
                It.IsAny<Func<IQueryable<ShoppingListItem>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ShoppingListItem, object>>?>(),
                false,
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ShoppingListItem?)null);

        _mockCache
            .Setup(c => c.GetAsync<Ingredient>($"ingredient:{ingredientId}"))
            .ReturnsAsync(ingredient);

        var notification = new IngredientDepletedEvent
        {
            UserId = userId,
            MasterIngredientId = ingredientId,
            IngredientName = "Süt"
        };

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert: DB'ye gidilmemeli, AddAsync bir kez çağrılmalı
        _mockIngredientRepo.Verify(
            r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(),
                It.IsAny<Func<IQueryable<Ingredient>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Ingredient, object>>?>(),
                false,
                false,
                It.IsAny<CancellationToken>()),
            Times.Never);

        _mockShoppingListRepo.Verify(
            r => r.AddAsync(
                It.Is<ShoppingListItem>(s =>
                    s.UserId == userId &&
                    s.IngredientId == ingredientId &&
                    s.Unit == MeasurementUnit.Liter),   // Volume → Liter
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenIngredientNotInRedis_ShouldFallbackToDatabase_AndCacheResult()
    {
        // Arrange: Redis'te yok, DB'de var
        var userId = Guid.NewGuid();
        var ingredientId = Guid.NewGuid();
        var ingredient = new Ingredient(ingredientId, "Un", Guid.NewGuid(), MeasurementCategory.Weight);

        _mockShoppingListRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<ShoppingListItem, bool>>>(),
                It.IsAny<Func<IQueryable<ShoppingListItem>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ShoppingListItem, object>>?>(),
                false,
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ShoppingListItem?)null);

        _mockCache
            .Setup(c => c.GetAsync<Ingredient>($"ingredient:{ingredientId}"))
            .ReturnsAsync((Ingredient?)null);

        _mockIngredientRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<Ingredient, bool>>>(),
                It.IsAny<Func<IQueryable<Ingredient>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Ingredient, object>>?>(),
                false,
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredient);

        var notification = new IngredientDepletedEvent
        {
            UserId = userId,
            MasterIngredientId = ingredientId,
            IngredientName = "Un"
        };

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert: cache'e yazıldı
        _mockCache.Verify(
            c => c.SetAsync(
                $"ingredient:{ingredientId}",
                ingredient,
                null,
                It.IsAny<TimeSpan?>()),
            Times.Once);

        // Assert: correct default unit (Weight → Gram)
        _mockShoppingListRepo.Verify(
            r => r.AddAsync(
                It.Is<ShoppingListItem>(s => s.Unit == MeasurementUnit.Gram),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenIngredientNotFoundAnywhere_ShouldDoNothing()
    {
        // Arrange: ne Redis'te ne DB'de var
        var ingredientId = Guid.NewGuid();

        _mockShoppingListRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<ShoppingListItem, bool>>>(),
                It.IsAny<Func<IQueryable<ShoppingListItem>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ShoppingListItem, object>>?>(),
                false,
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ShoppingListItem?)null);

        _mockCache
            .Setup(c => c.GetAsync<Ingredient>($"ingredient:{ingredientId}"))
            .ReturnsAsync((Ingredient?)null);

        _mockIngredientRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<Ingredient, bool>>>(),
                It.IsAny<Func<IQueryable<Ingredient>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Ingredient, object>>?>(),
                false,
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ingredient?)null);

        var notification = new IngredientDepletedEvent
        {
            UserId = Guid.NewGuid(),
            MasterIngredientId = ingredientId,
            IngredientName = "Bilinmeyen"
        };

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert: hiçbir şey eklenmemeli
        _mockShoppingListRepo.Verify(
            r => r.AddAsync(It.IsAny<ShoppingListItem>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
