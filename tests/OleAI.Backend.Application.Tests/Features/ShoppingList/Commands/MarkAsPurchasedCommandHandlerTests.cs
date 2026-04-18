using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.ShoppingList.Commands.MarkAsPurchased;
using OleAI.Backend.Application.Features.ShoppingList.Constants;
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
    private readonly ShoppingListBusinessRules _rules;
    private readonly MarkAsPurchasedCommandHandler _handler;

    public MarkAsPurchasedCommandHandlerTests()
    {
        _mockShoppingListRepo = new Mock<IShoppingListItemRepository>();
        _mockIngredientRepo = new Mock<IIngredientRepository>();

        _rules = new ShoppingListBusinessRules(
            _mockShoppingListRepo.Object,
            _mockIngredientRepo.Object);

        _handler = new MarkAsPurchasedCommandHandler(
            _mockShoppingListRepo.Object,
            _rules);
    }

    [Fact]
    public async Task Handle_WhenItemExists_ShouldMarkAsPurchasedAndReturnMessage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var item = new ShoppingListItem(userId, Guid.NewGuid(), 2, MeasurementUnit.Piece);

        _mockShoppingListRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<ShoppingListItem, bool>>>(),
                It.IsAny<Func<IQueryable<ShoppingListItem>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ShoppingListItem, object>>?>(),
                false,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        _mockShoppingListRepo
            .Setup(r => r.UpdateAsync(item, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        var command = new MarkAsPurchasedCommand { ItemId = itemId, UserId = userId };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(ShoppingListMessages.ItemMarkedAsPurchased);
        item.IsPurchased.Should().BeTrue();
        _mockShoppingListRepo.Verify(
            r => r.UpdateAsync(item, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenItemNotFound_ShouldThrowNotFoundException()
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

        var command = new MarkAsPurchasedCommand
        {
            ItemId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        // Act & Assert
        await FluentActions.Invoking(() => _handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage(ShoppingListMessages.ItemNotFoundOrUnauthorized);
    }
}
