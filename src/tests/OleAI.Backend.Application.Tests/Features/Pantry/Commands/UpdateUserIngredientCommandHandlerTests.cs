using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Pantry.Commands.UpdateUserIngredient;
using OleAI.Backend.Application.Features.Pantry.Constants;
using OleAI.Backend.Application.Features.Pantry.Dtos.Requests;
using OleAI.Backend.Application.Features.Pantry.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using System.Linq.Expressions;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Pantry.Commands
{
    public class UpdateUserIngredientCommandHandlerTests
    {
        private readonly Mock<IUserIngredientRepository> _mockUserIngredientRepository;
        private readonly Mock<IIngredientRepository> _mockIngredientRepository;
        private readonly Mock<IIngredientRequestRepository> _mockIngredientRequestRepository;
        private readonly PantryBusinessRules _businessRules;
        private readonly UpdateUserIngredientCommandHandler _handler;

        public UpdateUserIngredientCommandHandlerTests()
        {
            _mockUserIngredientRepository = new Mock<IUserIngredientRepository>();
            _mockIngredientRepository = new Mock<IIngredientRepository>();
            _mockIngredientRequestRepository = new Mock<IIngredientRequestRepository>();

            _businessRules = new PantryBusinessRules(
                _mockIngredientRepository.Object,
                _mockUserIngredientRepository.Object,
                _mockIngredientRequestRepository.Object);

            _handler = new UpdateUserIngredientCommandHandler(_mockUserIngredientRepository.Object, _businessRules);
        }

        [Fact]
        public async Task Handle_WhenValidRequest_ShouldUpdateQuantityAndReturnMessage()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var ingredientId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var request = new UpdateUserIngredientRequest { Id = itemId, UserId = userId, NewQuantity = 10.5 };
            var command = new UpdateUserIngredientCommand { Request = request };

            var existingItem = UserIngredient.CreateStandard(itemId, userId, ingredientId, 5.0, null);

            // Mock GetAsync to return the existing item
            _mockUserIngredientRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<UserIngredient, bool>>>(),
                    null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingItem);

            _mockUserIngredientRepository.Setup(r => r.UpdateAsync(existingItem, It.IsAny<CancellationToken>())).ReturnsAsync(existingItem);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(PantryMessages.IngredientQuantityUpdatedMessage);
            existingItem.Quantity.Should().Be(10.5);

            _mockUserIngredientRepository.Verify(r => r.UpdateAsync(existingItem, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenItemNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var command = new UpdateUserIngredientCommand 
            { 
                Request = new UpdateUserIngredientRequest { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), NewQuantity = 5.0 } 
            };

            // First GetAsync for the handler
            _mockUserIngredientRepository
                .SetupSequence(r => r.GetAsync(
                    It.IsAny<Expression<Func<UserIngredient, bool>>>(),
                    null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserIngredient)null!) // Handler gets null
                .ReturnsAsync((UserIngredient)null!); // Business rule checks again and gets null

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage(PantryMessages.InventoryItemNotFoundMessage);

            _mockUserIngredientRepository.Verify(r => r.UpdateAsync(It.IsAny<UserIngredient>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
