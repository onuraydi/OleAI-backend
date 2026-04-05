using FluentAssertions;
using OleAI.Backend.Domain;
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

        [Fact]
        public void UpdateQuantity_Should_ThrowException_When_NewQuantityIsNegative()
        {
            // Arrange
            var userIngredient = UserIngredient.CreateStandard(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 100);

            // Act
            Action action = () => userIngredient.UpdateQuantity(-1);

            // Assert
            action.Should().Throw<ArgumentException>().WithMessage(DomainConstants.AmountCannotBeNegativeMessage);
        }

        [Fact]
        public void ApprovePendingRequest_Should_ThrowException_When_RequestIdIsNull()
        {
            // Arrange
            var approvedIngredient = UserIngredient.CreateStandard(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 1);

            // Act
            Action action = () => approvedIngredient.ApprovePendingRequest(Guid.NewGuid());

            // Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage(DomainConstants.ProductAlreadyApprovedOrRequestIdMissingMessage);
        }
    }
}
