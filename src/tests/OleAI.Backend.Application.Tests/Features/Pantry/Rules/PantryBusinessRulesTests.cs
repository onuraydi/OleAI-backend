using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Pantry.Constants;
using OleAI.Backend.Application.Features.Pantry.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using System.Linq.Expressions;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;

namespace OleAI.Backend.Application.Tests.Features.Pantry.Rules
{
    public class PantryBusinessRulesTests
    {
        private readonly Mock<IIngredientRepository> _mockIngredientRepository;
        private readonly Mock<IUserIngredientRepository> _mockUserIngredientRepository;
        private readonly Mock<IIngredientRequestRepository> _mockIngredientRequestRepository;
        private readonly PantryBusinessRules _businessRules;

        public PantryBusinessRulesTests()
        {
            _mockIngredientRepository = new Mock<IIngredientRepository>();
            _mockUserIngredientRepository = new Mock<IUserIngredientRepository>();
            _mockIngredientRequestRepository = new Mock<IIngredientRequestRepository>();

            _businessRules = new PantryBusinessRules(
                _mockIngredientRepository.Object,
                _mockUserIngredientRepository.Object,
                _mockIngredientRequestRepository.Object);
        }

        [Fact]
        public async Task IngredientMustExistWhenSelectedFromList_WhenIngredientNotFound_ShouldThrowBusinessException()
        {
            // Arrange
            var ingredientId = Guid.NewGuid();
            _mockIngredientRepository
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Ingredient)null!);

            // Act
            Func<Task> act = async () => await _businessRules.IngredientMustExistWhenSelectedFromList(ingredientId);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage(PantryMessages.IngredientNotFoundMessage);
        }

        [Fact]
        public async Task IngredientMustExistWhenSelectedFromList_WhenIngredientFound_ShouldNotThrowException()
        {
            // Arrange
            var ingredientId = Guid.NewGuid();
            _mockIngredientRepository
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Ingredient { Id = ingredientId });

            // Act
            Func<Task> act = async () => await _businessRules.IngredientMustExistWhenSelectedFromList(ingredientId);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public void CannotSendBothIdAndName_WhenBothProvided_ShouldThrowBusinessException()
        {
            // Act
            Action act = () => _businessRules.CannotSendBothIdAndName(Guid.NewGuid(), "Milk");

            // Assert
            act.Should().Throw<BusinessException>()
                .WithMessage(PantryMessages.ProductSelectionConflictMessage);
        }

        [Fact]
        public void CannotSendBothIdAndName_WhenNeitherProvided_ShouldThrowBusinessException()
        {
            // Act
            Action act = () => _businessRules.CannotSendBothIdAndName(null, "");

            // Assert
            act.Should().Throw<BusinessException>()
                .WithMessage(PantryMessages.ProductSelectionRequiredMessage);
        }

        [Fact]
        public void CannotSendBothIdAndName_WhenOnlyIdProvided_ShouldNotThrowException()
        {
            // Act
            Action act = () => _businessRules.CannotSendBothIdAndName(Guid.NewGuid(), "");

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void CannotSendBothIdAndName_WhenOnlyNameProvided_ShouldNotThrowException()
        {
            // Act
            Action act = () => _businessRules.CannotSendBothIdAndName(null, "Milk");

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public async Task InventoryItemNotFound_WhenNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var inventoryId = Guid.NewGuid();
            _mockUserIngredientRepository
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<UserIngredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserIngredient)null!);

            // Act
            Func<Task> act = async () => await _businessRules.InventoryItemNotFound(inventoryId);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage(PantryMessages.InventoryItemNotFoundMessage);
        }

        [Fact]
        public void IngredientRequestMustExistWhenSelected_WhenNull_ShouldThrowNotFoundException()
        {
            // Act
            Action act = () => _businessRules.IngredientRequestMustExistWhenSelected(null);

            // Assert
            act.Should().Throw<NotFoundException>()
                .WithMessage(PantryMessages.IngredientRequestNotFoundMessage);
        }

        [Fact]
        public void CannotBeCancelledRequest_WhenNotPending_ShouldThrowBusinessException()
        {
            // Arrange
            var status = IngredientRequestStatus.Approved;

            // Act
            Action act = () => _businessRules.CannotBeCancelledRequest(status);

            // Assert
            act.Should().Throw<BusinessException>()
                .WithMessage(PantryMessages.RequestAlreadyProcessedCannotBeCancelledMessage);
        }

        [Fact]
        public void CannotBeCancelledRequest_WhenPending_ShouldNotThrowException()
        {
            // Arrange
            var status = IngredientRequestStatus.Pending;

            // Act
            Action act = () => _businessRules.CannotBeCancelledRequest(status);

            // Assert
            act.Should().NotThrow();
        }
    }
}
