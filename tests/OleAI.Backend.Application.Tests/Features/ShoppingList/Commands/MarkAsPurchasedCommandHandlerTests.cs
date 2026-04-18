using System.Linq.Expressions;
using FluentAssertions;
using MediatR;
using Moq;
using OleAI.Backend.Application.Features.ShoppingList.Commands.MarkAsPurchased;
using OleAI.Backend.Application.Features.ShoppingList.Constants;
using OleAI.Backend.Application.Features.ShoppingList.Events;
using OleAI.Backend.Application.Features.ShoppingList.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.ShoppingList.Commands;

public class MarkAsPurchasedCommandHandlerTests
{
    private readonly Mock<IShoppingListItemRepository> _mockShoppingListRepo;
    private readonly Mock<IIngredientRepository> _mockIngredientRepo;
    private readonly Mock<IMediator> _mockMediator;
    private readonly ShoppingListBusinessRules _rules;
    private readonly MarkAsPurchasedCommandHandler _handler;

    public MarkAsPurchasedCommandHandlerTests()
    {
        _mockShoppingListRepo = new Mock<IShoppingListItemRepository>();
        _mockIngredientRepo = new Mock<IIngredientRepository>();
        _mockMediator = new Mock<IMediator>();

        _rules = new ShoppingListBusinessRules(
            _mockShoppingListRepo.Object,
            _mockIngredientRepo.Object);

        _handler = new MarkAsPurchasedCommandHandler(
            _mockShoppingListRepo.Object,
            _rules,
            _mockMediator.Object);
    }

    [Fact]
    public async Task Handle_WhenItemExists_ShouldMarkAsPurchased_AndPublishEvent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ingredientId = Guid.NewGuid();
        var item = new ShoppingListItem(userId, ingredientId, 2, MeasurementUnit.Piece);

        _mockShoppingListRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<ShoppingListItem, bool>>>(),
                null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        _mockShoppingListRepo
            .Setup(r => r.UpdateAsync(item, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        var command = new MarkAsPurchasedCommand { ItemId = item.Id, UserId = userId };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(ShoppingListMessages.ItemMarkedAsPurchased);
        item.IsPurchased.Should().BeTrue();

        // Event yayınlandı mı?
        _mockMediator.Verify(
            m => m.Publish(
                It.Is<ItemPurchasedEvent>(e =>
                    e.UserId == userId &&
                    e.IngredientId == ingredientId &&
                    e.Quantity == 2 &&
                    e.Unit == MeasurementUnit.Piece),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenItemNotFound_ShouldThrowNotFoundException()
    {
        _mockShoppingListRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<ShoppingListItem, bool>>>(),
                null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ShoppingListItem?)null);

        var command = new MarkAsPurchasedCommand
        {
            ItemId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        await FluentActions.Invoking(() => _handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage(ShoppingListMessages.ItemNotFoundOrUnauthorized);

        // Event yayınlanmamalı
        _mockMediator.Verify(
            m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
