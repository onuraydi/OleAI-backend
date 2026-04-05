using FluentAssertions;
using MediatR;
using Moq;
using OleAI.Backend.Application.Features.Pantry.Commands.AddUserIngredient;
using OleAI.Backend.Application.Features.Pantry.Constants;
using OleAI.Backend.Application.Features.Pantry.Dtos.Requests;
using OleAI.Backend.Application.Features.Pantry.Events;
using OleAI.Backend.Application.Features.Pantry.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using System.Linq.Expressions;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;

namespace OleAI.Backend.Application.Tests.Features.Pantry.Commands
{
    public class AddUserIngredientCommandHandlerTests
    {
        private readonly Mock<IUserIngredientRepository> _mockUserIngredientRepository;
        private readonly Mock<IIngredientRequestRepository> _mockIngredientRequestRepository;
        private readonly Mock<IIngredientRepository> _mockIngredientRepository;
        private readonly Mock<IMediator> _mockMediator;
        private readonly PantryBusinessRules _businessRules;
        private readonly AddUserIngredientCommandHandler _handler;

        public AddUserIngredientCommandHandlerTests()
        {
            _mockUserIngredientRepository = new Mock<IUserIngredientRepository>();
            _mockIngredientRequestRepository = new Mock<IIngredientRequestRepository>();
            _mockIngredientRepository = new Mock<IIngredientRepository>();
            _mockMediator = new Mock<IMediator>();

            _businessRules = new PantryBusinessRules(
                _mockIngredientRepository.Object,
                _mockUserIngredientRepository.Object,
                _mockIngredientRequestRepository.Object);

            _handler = new AddUserIngredientCommandHandler(
                _mockUserIngredientRepository.Object,
                _mockIngredientRequestRepository.Object,
                _businessRules,
                _mockMediator.Object);
        }

        [Fact]
        public async Task Handle_WhenIngredientSelectedFromListAndAlreadyInPantry_ShouldUpdateQuantity()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var ingredientId = Guid.NewGuid();
            var request = new AddUserIngredientRequest { UserId = userId, IngredientId = ingredientId, Quantity = 2.5 };
            var command = new AddUserIngredientCommand { Request = request };

            var existingItem = UserIngredient.CreateStandard(Guid.NewGuid(), userId, ingredientId, 1.0, null);

            // Mock Rule: IngredientMustExistWhenSelectedFromList
            _mockIngredientRepository
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Ingredient { Id = ingredientId });

            // Mock: GetAsync for existing item
            _mockUserIngredientRepository
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<UserIngredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingItem);
            
            _mockUserIngredientRepository
                .Setup(r => r.UpdateAsync(existingItem, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingItem);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be(PantryMessages.IngredientQuantityUpdatedMessage);
            result.IsPendingApproval.Should().BeFalse();
            existingItem.Quantity.Should().Be(3.5); // 1.0 + 2.5

            _mockUserIngredientRepository.Verify(r => r.UpdateAsync(existingItem, It.IsAny<CancellationToken>()), Times.Once);
            _mockUserIngredientRepository.Verify(r => r.AddAsync(It.IsAny<UserIngredient>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenIngredientSelectedFromListAndNotInPantry_ShouldCreateNewStandardItem()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var ingredientId = Guid.NewGuid();
            var request = new AddUserIngredientRequest { UserId = userId, IngredientId = ingredientId, Quantity = 2.0 };
            var command = new AddUserIngredientCommand { Request = request };

            // Mock Rule: IngredientMustExistWhenSelectedFromList
            _mockIngredientRepository
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Ingredient { Id = ingredientId });

            // Mock: GetAsync for existing item returns null
            _mockUserIngredientRepository
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<UserIngredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserIngredient)null!);

            _mockUserIngredientRepository
                .Setup(r => r.AddAsync(It.IsAny<UserIngredient>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(It.IsAny<UserIngredient>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be(PantryMessages.IngredientAddedPantryMessage);
            result.IsPendingApproval.Should().BeFalse();

            _mockUserIngredientRepository.Verify(r => r.AddAsync(It.Is<UserIngredient>(ui => ui.IngredientId == ingredientId), It.IsAny<CancellationToken>()), Times.Once);
            _mockUserIngredientRepository.Verify(r => r.UpdateAsync(It.IsAny<UserIngredient>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenRequestedNameProvided_ShouldCreatePendingItemAndPublishEvent()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var requestedName = "New Exotic Fruit";
            var request = new AddUserIngredientRequest { UserId = userId, RequestedName = requestedName, Quantity = 5.0 };
            var command = new AddUserIngredientCommand { Request = request };

            UserIngredient pendingItem = null!;

            _mockIngredientRequestRepository
                .Setup(r => r.AddAsync(It.IsAny<IngredientRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(It.IsAny<IngredientRequest>());

            _mockUserIngredientRepository
                .Setup(r => r.AddAsync(It.IsAny<UserIngredient>(), It.IsAny<CancellationToken>()))
                .Callback<UserIngredient, CancellationToken>((ui, ct) => pendingItem = ui)
                .ReturnsAsync(It.IsAny<UserIngredient>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be(PantryMessages.IngredientSentForAiApprovalMessage);
            result.IsPendingApproval.Should().BeTrue();

            _mockIngredientRequestRepository.Verify(r => r.AddAsync(It.Is<IngredientRequest>(ir => ir.RequestedName == requestedName), It.IsAny<CancellationToken>()), Times.Once);
            _mockUserIngredientRepository.Verify(r => r.AddAsync(It.IsAny<UserIngredient>(), It.IsAny<CancellationToken>()), Times.Once);
            
            _mockMediator.Verify(m => m.Publish(It.Is<IngredientRequestedEvent>(e => e.RequestedName == requestedName), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenBothProvided_ShouldThrowBusinessException()
        {
            // Arrange
            var request = new AddUserIngredientRequest { UserId = Guid.NewGuid(), IngredientId = Guid.NewGuid(), RequestedName = "Oranges", Quantity = 2.0 };
            var command = new AddUserIngredientCommand { Request = request };

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BusinessException>().WithMessage(PantryMessages.ProductSelectionConflictMessage);
        }
    }
}
