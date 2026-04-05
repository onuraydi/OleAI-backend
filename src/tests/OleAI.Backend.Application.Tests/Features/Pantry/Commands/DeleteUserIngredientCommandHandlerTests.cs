using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Pantry.Commands.DeleteUserIngredient;
using OleAI.Backend.Application.Features.Pantry.Constants;
using OleAI.Backend.Application.Features.Pantry.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using System.Linq.Expressions;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Pantry.Commands
{
    public class DeleteUserIngredientCommandHandlerTests
    {
        private readonly Mock<IUserIngredientRepository> _mockUserIngredientRepository;
        private readonly Mock<IIngredientRepository> _mockIngredientRepository;
        private readonly Mock<IIngredientRequestRepository> _mockIngredientRequestRepository;
        private readonly PantryBusinessRules _businessRules;
        private readonly DeleteUserIngredientCommandHandler _handler;

        public DeleteUserIngredientCommandHandlerTests()
        {
            _mockUserIngredientRepository = new Mock<IUserIngredientRepository>();
            _mockIngredientRepository = new Mock<IIngredientRepository>();
            _mockIngredientRequestRepository = new Mock<IIngredientRequestRepository>();

            _businessRules = new PantryBusinessRules(
                _mockIngredientRepository.Object,
                _mockUserIngredientRepository.Object,
                _mockIngredientRequestRepository.Object);

            _handler = new DeleteUserIngredientCommandHandler(_mockUserIngredientRepository.Object, _businessRules);
        }

        [Fact]
        public async Task Handle_WhenValidRequest_ShouldDeleteAndReturnMessage()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var command = new DeleteUserIngredientCommand { Id = itemId, UserId = userId };

            var existingItem = UserIngredient.CreateStandard(itemId, userId, Guid.NewGuid(), 5.0, null);

            // Mock GetAsync to return the existing item
            _mockUserIngredientRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<UserIngredient, bool>>>(),
                    null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingItem);
            
            _mockUserIngredientRepository.Setup(r => r.DeleteAsync(existingItem, false, It.IsAny<CancellationToken>())).ReturnsAsync(existingItem);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(PantryMessages.IngredientDeletedMessage);

            _mockUserIngredientRepository.Verify(r => r.DeleteAsync(existingItem, false, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenItemNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var command = new DeleteUserIngredientCommand { Id = Guid.NewGuid(), UserId = Guid.NewGuid() };

            // Mock GetAsync for the handler to return null
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

            _mockUserIngredientRepository.Verify(r => r.DeleteAsync(It.IsAny<UserIngredient>(), false, It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
