using FluentAssertions;
using MediatR;
using Moq;
using OleAI.Backend.Application.Features.Pantry.EventHandlers;
using OleAI.Backend.Application.Features.Pantry.Events;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Pantry.EventHandlers;

public class InventoryDeductionEventHandlerTests
{
    private readonly Mock<IUserIngredientRepository> _mockUserIngredientRepository;
    private readonly Mock<IMediator> _mockMediator;
    private readonly InventoryDeductionEventHandler _handler;

    public InventoryDeductionEventHandlerTests()
    {
        _mockUserIngredientRepository = new Mock<IUserIngredientRepository>();
        _mockMediator = new Mock<IMediator>();
        _handler = new InventoryDeductionEventHandler(_mockUserIngredientRepository.Object, _mockMediator.Object);
    }

    [Fact]
    public async Task Handle_WhenQuantityIsSufficient_ShouldConsumeAndUpdate()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ingredientId = Guid.NewGuid();

        var pantryItem = UserIngredient.CreateStandard(Guid.NewGuid(), userId, ingredientId, 10, MeasurementUnit.Gram);

        var notification = new RecipeConsumedEvent
        {
            UserId = userId,
            RecipeId = Guid.NewGuid(),
            ConsumedIngredients = new Dictionary<Guid, double> { [ingredientId] = 3 }
        };

        _mockUserIngredientRepository
            .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<UserIngredient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Paginate<UserIngredient> { Items = new List<UserIngredient> { pantryItem } });

        _mockUserIngredientRepository
            .Setup(r => r.UpdateAsync(pantryItem, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pantryItem);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        pantryItem.Amount.Should().Be(7);
        _mockUserIngredientRepository.Verify(r => r.UpdateAsync(pantryItem, It.IsAny<CancellationToken>()), Times.Once);
        _mockUserIngredientRepository.Verify(r => r.DeleteAsync(It.IsAny<UserIngredient>(), false, It.IsAny<CancellationToken>()), Times.Never);
        _mockMediator.Verify(m => m.Publish(It.IsAny<IngredientDepletedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenQuantityIsInsufficient_ShouldDeleteAndPublishDepletedEvent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ingredientId = Guid.NewGuid();

        var pantryItem = UserIngredient.CreateStandard(Guid.NewGuid(), userId, ingredientId, 2, MeasurementUnit.Gram);

        var notification = new RecipeConsumedEvent
        {
            UserId = userId,
            RecipeId = Guid.NewGuid(),
            ConsumedIngredients = new Dictionary<Guid, double> { [ingredientId] = 3 }
        };

        _mockUserIngredientRepository
            .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<UserIngredient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Paginate<UserIngredient> { Items = new List<UserIngredient> { pantryItem } });

        _mockUserIngredientRepository
            .Setup(r => r.DeleteAsync(pantryItem, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pantryItem);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _mockUserIngredientRepository.Verify(r => r.DeleteAsync(pantryItem, false, It.IsAny<CancellationToken>()), Times.Once);
        _mockUserIngredientRepository.Verify(r => r.UpdateAsync(It.IsAny<UserIngredient>(), It.IsAny<CancellationToken>()), Times.Never);

        _mockMediator.Verify(m => m.Publish(
            It.Is<IngredientDepletedEvent>(e => e.UserId == userId && e.MasterIngredientId == ingredientId),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
