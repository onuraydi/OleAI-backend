using OleAI.Backend.Application.Features.Pantry.EventHandlers;
using OleAI.Backend.Application.Features.Pantry.Events;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Pantry.EventHandlers;

public class ShoppingListEventHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCompleteWithoutThrowing()
    {
        // Arrange
        var handler = new ShoppingListEventHandler();
        var notification = new IngredientDepletedEvent
        {
            UserId = Guid.NewGuid(),
            MasterIngredientId = Guid.NewGuid(),
            IngredientName = "Milk"
        };

        // Act
        var action = async () => await handler.Handle(notification, CancellationToken.None);

        // Assert
        await action();
    }
}
