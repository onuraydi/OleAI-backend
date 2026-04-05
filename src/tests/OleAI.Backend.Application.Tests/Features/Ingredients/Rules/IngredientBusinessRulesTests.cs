using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Ingredients.Constants;
using OleAI.Backend.Application.Features.Ingredients.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using System.Linq.Expressions;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;

namespace OleAI.Backend.Application.Tests.Features.Ingredients.Rules
{
    public class IngredientBusinessRulesTests
    {
        private readonly Mock<IIngredientRepository> _mockIngredientRepository;
        private readonly IngredientBusinessRules _businessRules;

        public IngredientBusinessRulesTests()
        {
            _mockIngredientRepository = new Mock<IIngredientRepository>();
            _businessRules = new IngredientBusinessRules(_mockIngredientRepository.Object);
        }

        [Fact]
        public void IngredientMustExistWhenRequested_WhenIngredientIsNull_ShouldThrowNotFoundException()
        {
            // Act
            Action act = () => _businessRules.IngredientMustExistWhenRequested(null);

            // Assert
            act.Should().Throw<NotFoundException>()
                .WithMessage(IngredientMessages.IngredientNotFoundMessage);
        }

        [Fact]
        public void IngredientMustExistWhenRequested_WhenIngredientIsNotNull_ShouldNotThrowException()
        {
            // Arrange
            var ingredient = new Ingredient { Id = Guid.NewGuid(), Name = "Tomato" };

            // Act
            Action act = () => _businessRules.IngredientMustExistWhenRequested(ingredient);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public async Task IngredientNameCannotBeDuplicatedWhenInserted_WhenNameExistsAndNotDeleted_ShouldThrowBusinessException()
        {
            // Arrange
            var ingredientName = "Tomato";
            _mockIngredientRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Ingredient, bool>>>(),
                    It.IsAny<Func<IQueryable<Ingredient>, IIncludableQueryable<Ingredient, object>>?>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Ingredient { Id = Guid.NewGuid(), Name = "Tomato", DeletedDate = null });

            // Act
            Func<Task> act = async () => await _businessRules.IngredientNameCannotBeDuplicatedWhenInserted(ingredientName);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage(IngredientMessages.ActiveIngredientAlreadyExistsMessage);
        }

        [Fact]
        public async Task IngredientNameCannotBeDuplicatedWhenInserted_WhenNameDoesNotExist_ShouldNotThrowException()
        {
            // Arrange
            var ingredientName = "Tomato";
            _mockIngredientRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Ingredient, bool>>>(),
                    It.IsAny<Func<IQueryable<Ingredient>, IIncludableQueryable<Ingredient, object>>?>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Ingredient)null!);

            // Act
            Func<Task> act = async () => await _businessRules.IngredientNameCannotBeDuplicatedWhenInserted(ingredientName);

            // Assert
            await act.Should().NotThrowAsync();
        }
    }
}
