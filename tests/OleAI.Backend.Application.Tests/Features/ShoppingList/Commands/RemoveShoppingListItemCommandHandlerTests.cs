using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.ShoppingList.Commands.RemoveShoppingListItem;
using OleAI.Backend.Application.Features.ShoppingList.Constants;
using OleAI.Backend.Application.Features.ShoppingList.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.ShoppingList.Commands;

public class RemoveShoppingListItemCommandHandlerTests
{
    private readonly Mock<IShoppingListItemRepository> _mockShoppingListRepo;
    private readonly Mock<IIngredientRepository> _mockIngredientRepo;
    private readonly ShoppingListBusinessRules _rules;
    private readonly RemoveShoppingListItemCommandHandler _handler;

    public RemoveShoppingListItemCommandHandlerTests()
    {
        _mockShoppingListRepo = new Mock<IShoppingListItemRepository>();
        _mockIngredientRepo = new Mock<IIngredientRepository>();

        _rules = new ShoppingListBusinessRules(
            _mockShoppingListRepo.Object,
            _mockIngredientRepo.Object);

        _handler = new RemoveShoppingListItemCommandHandler(
            _mockShoppingListRepo.Object,
            _rules);
    }

    [Fact]
    public async Task Handle_WhenItemExists_ShouldDeleteAndReturnMessage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var item = new ShoppingListItem(userId, Guid.NewGuid(), 1, MeasurementUnit.Piece);

        _mockShoppingListRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<ShoppingListItem, bool>>>(),
                It.IsAny<Func<IQueryable<ShoppingListItem>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ShoppingListItem, object>>?>(),
                false,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        _mockShoppingListRepo
            .Setup(r => r.DeleteAsync(item, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        var command = new RemoveShoppingListItemCommand { ItemId = itemId, UserId = userId };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(ShoppingListMessages.ItemRemovedFromList);
        _mockShoppingListRepo.Verify(
            r => r.DeleteAsync(item, true, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenItemBelongsToDifferentUser_ShouldThrowNotFoundException()
    {
        // Arrange: repo returns null (yabancı userId filtresine takıldı)
        _mockShoppingListRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<ShoppingListItem, bool>>>(),
                It.IsAny<Func<IQueryable<ShoppingListItem>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ShoppingListItem, object>>?>(),
                false,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ShoppingListItem?)null);

        var command = new RemoveShoppingListItemCommand
        {
            ItemId = Guid.NewGuid(),
            UserId = Guid.NewGuid()     // farklı kullanıcı
        };

        // Act & Assert
        await FluentActions.Invoking(() => _handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage(ShoppingListMessages.ItemNotFoundOrUnauthorized);
    }
}
