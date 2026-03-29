using FluentAssertions;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Domain.Tests.Entities
{
    public class UserIngredientTests
    {
        [Fact]
        public void Consume_Should_DevreaseQuantity_When_AmountIsValid()
        {
            // Arrange
            var userIngredient = UserIngredient.CreateStandard(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 1000);

            // Act
            userIngredient.Consume(250);

            // Assert
            userIngredient.Quantity.Should().Be(750);
        }

        [Fact]
        public void Consume_Should_ThrowException_When_NotEnoughtQuantity()
        {
            // Arrange
            var userIngredient = UserIngredient.CreateStandard(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 100);

            // Act
            Action action = () => userIngredient.Consume(250);

            // Assert
            action.Should().Throw<InvalidOperationException>().WithMessage("Dolapta yeterli miktar yok.");
        }

        [Fact]
        public void AddQuantity_Should_IncreaseQuantity_When_AmountIsValid()
        {
            // Arrange
            var userIngredient = UserIngredient.CreateStandard(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 500);

            // Act
            userIngredient.AddQuantity(200);

            // Assert
            userIngredient.Quantity.Should().Be(700);
        }

        [Fact]
        public void ApprovePendingRequest_Should_SetIngredientId_When_Valid()
        {
            // Arrange
            var pendingId = Guid.NewGuid();
            var userIngredient = UserIngredient.CreatePending(Guid.NewGuid(), Guid.NewGuid(), pendingId, 1);
            var masterIngredientId = Guid.NewGuid();

            // Act
            userIngredient.ApprovePendingRequest(masterIngredientId);

            // Assert
            userIngredient.IngredientId.Should().Be(masterIngredientId);
        }
    }
}
