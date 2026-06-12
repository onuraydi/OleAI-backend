using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Pantry.Dtos;
using OleAI.Backend.Application.Features.ShoppingList.Commands.UpdateShoppingListItem;
using OleAI.Backend.Application.Features.ShoppingList.Constants;
using OleAI.Backend.Application.Features.ShoppingList.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.ShoppingList.Commands;

public class UpdateShoppingListItemCommandHandlerTests
{
    private readonly Mock<IShoppingListItemRepository> _mockShoppingListRepo;
    private readonly Mock<IIngredientRepository> _mockIngredientRepo;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly ShoppingListBusinessRules _rules;
    private readonly UpdateShoppingListItemCommandHandler _handler;

    public UpdateShoppingListItemCommandHandlerTests()
    {
        _mockShoppingListRepo = new Mock<IShoppingListItemRepository>();
        _mockIngredientRepo = new Mock<IIngredientRepository>();
        _mockCacheService = new Mock<ICacheService>();

        _rules = new ShoppingListBusinessRules(
            _mockShoppingListRepo.Object,
            _mockIngredientRepo.Object);

        _handler = new UpdateShoppingListItemCommandHandler(
            _mockShoppingListRepo.Object,
            _rules,
            _mockCacheService.Object);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ShouldUpdateAndReturnMessage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var item = ShoppingListItem.CreateStandard(userId, Guid.NewGuid(), 1, MeasurementUnit.Piece);

        _mockShoppingListRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<ShoppingListItem, bool>>>(),
                null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        _mockShoppingListRepo
            .Setup(r => r.UpdateAsync(item, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        var command = new UpdateShoppingListItemCommand
        {
            ItemId = item.Id,
            UserId = userId,
            NewQuantity = 3,
            NewUnit = MeasurementUnit.Piece
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(ShoppingListMessages.ItemUpdated);
        item.Quantity.Should().BeApproximately(3d, 0.0001d);
        _mockShoppingListRepo.Verify(
            r => r.UpdateAsync(item, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenItemAlreadyPurchased_ShouldThrowBusinessException()
    {
        // Arrange: item satın alınmış (domain guard devreye girer)
        var userId = Guid.NewGuid();
        var item = ShoppingListItem.CreateStandard(userId, Guid.NewGuid(), 1, MeasurementUnit.Piece);
        item.MarkAsPurchased(); // IsPurchased = true

        _mockShoppingListRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<ShoppingListItem, bool>>>(),
                null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        var command = new UpdateShoppingListItemCommand
        {
            ItemId = item.Id,
            UserId = userId,
            NewQuantity = 5,
            NewUnit = MeasurementUnit.Piece
        };

        // Act & Assert: domain guard BusinessException fırlatır
        await FluentActions.Invoking(() => _handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<BusinessException>();
    }

    [Fact]
    public async Task Handle_WhenItemNotFound_ShouldThrowNotFoundException()
    {
        _mockShoppingListRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<ShoppingListItem, bool>>>(),
                null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ShoppingListItem?)null);

        var command = new UpdateShoppingListItemCommand
        {
            ItemId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            NewQuantity = 1,
            NewUnit = MeasurementUnit.Piece
        };

        await FluentActions.Invoking(() => _handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage(ShoppingListMessages.ItemNotFoundOrUnauthorized);
    }
}

