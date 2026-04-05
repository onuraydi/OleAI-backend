using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Ingredients.Commands.DeleteIngredient;
using OleAI.Backend.Application.Features.Ingredients.Constants;
using OleAI.Backend.Application.Features.Ingredients.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using System.Linq.Expressions;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;

namespace OleAI.Backend.Application.Tests.Features.Ingredients.Commands
{
    public class DeleteIngredientCommandHandlerTests
    {
        private readonly Mock<IIngredientRepository> _mockIngredientRepository;
        private readonly Mock<IUserIngredientRepository> _mockUserIngredientRepository;
        private readonly IngredientBusinessRules _businessRules;
        private readonly DeleteIngredientCommandHandler _handler;

        public DeleteIngredientCommandHandlerTests()
        {
            _mockIngredientRepository = new Mock<IIngredientRepository>();
            _mockUserIngredientRepository = new Mock<IUserIngredientRepository>();
            _businessRules = new IngredientBusinessRules(_mockIngredientRepository.Object);

            _handler = new DeleteIngredientCommandHandler(
                _mockIngredientRepository.Object, 
                _mockUserIngredientRepository.Object, 
                _businessRules);
        }

        [Fact]
        public async Task Handle_WhenIngredientNotUsedInPantry_ShouldHardDeleteAndReturnDeletionMessage()
        {
            // Arrange
            var ingredientId = Guid.NewGuid();
            var command = new DeleteIngredientCommand { Id = ingredientId };
            var existingIngredient = new Ingredient { Id = ingredientId, Name = "Tomato" };

            _mockIngredientRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Ingredient, bool>>>(),
                    null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingIngredient);

            var emptyUserIngredients = new Paginate<UserIngredient> { Items = new List<UserIngredient>() };

            _mockUserIngredientRepository
                .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<UserIngredient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyUserIngredients);

            _mockIngredientRepository.Setup(r => r.DeleteAsync(existingIngredient, true)).ReturnsAsync(existingIngredient);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(IngredientMessages.IngredientDeletedMessage);
            _mockIngredientRepository.Verify(r => r.DeleteAsync(existingIngredient, true), Times.Once); // permanent = true
        }

        [Fact]
        public async Task Handle_WhenIngredientIsUsedInPantry_ShouldSoftDeleteAndReturnSoftDeleteMessage()
        {
            // Arrange
            var ingredientId = Guid.NewGuid();
            var command = new DeleteIngredientCommand { Id = ingredientId };
            var existingIngredient = new Ingredient { Id = ingredientId, Name = "Tomato" };

            _mockIngredientRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Ingredient, bool>>>(),
                    null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingIngredient);

            var usedUserIngredients = new Paginate<UserIngredient> 
            { 
                Items = new List<UserIngredient> { new UserIngredient { Id = Guid.NewGuid(), IngredientId = ingredientId } } 
            };

            _mockUserIngredientRepository
                .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<UserIngredient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usedUserIngredients);

            _mockIngredientRepository.Setup(r => r.DeleteAsync(existingIngredient, false)).ReturnsAsync(existingIngredient);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(IngredientMessages.IngredientSoftDeletedDueToInventoryUsageMessage);
            _mockIngredientRepository.Verify(r => r.DeleteAsync(existingIngredient, false), Times.Once); // permanent = false
        }

        [Fact]
        public async Task Handle_WhenIngredientNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var command = new DeleteIngredientCommand { Id = Guid.NewGuid() };

            _mockIngredientRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Ingredient, bool>>>(),
                    null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Ingredient)null!);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage(IngredientMessages.IngredientNotFoundMessage);

            _mockUserIngredientRepository.Verify(r => r.GetListAsync(It.IsAny<QueryOptions<UserIngredient>>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockIngredientRepository.Verify(r => r.DeleteAsync(It.IsAny<Ingredient>(), It.IsAny<bool>()), Times.Never);
        }
    }
}
