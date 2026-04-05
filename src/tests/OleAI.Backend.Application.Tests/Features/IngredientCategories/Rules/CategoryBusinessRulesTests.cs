using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.IngredientCategories.Constants;
using OleAI.Backend.Application.Features.IngredientCategories.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using System.Linq.Expressions;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;

namespace OleAI.Backend.Application.Tests.Features.IngredientCategories.Rules
{
    public class CategoryBusinessRulesTests
    {
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<IIngredientRepository> _mockIngredientRepository;
        private readonly CategoryBusinessRules _businessRules;

        public CategoryBusinessRulesTests()
        {
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockIngredientRepository = new Mock<IIngredientRepository>();
            _businessRules = new CategoryBusinessRules(_mockCategoryRepository.Object, _mockIngredientRepository.Object);
        }

        [Fact]
        public void CategoryMustExistWhenRequested_WhenCategoryIsNull_ShouldThrowNotFoundException()
        {
            // Act
            Action act = () => _businessRules.CategoryMustExistWhenRequested(null);

            // Assert
            act.Should().Throw<NotFoundException>()
                .WithMessage(CategoryMessages.CategoryNotFoundMessage);
        }

        [Fact]
        public void CategoryMustExistWhenRequested_WhenCategoryIsNotNull_ShouldNotThrowException()
        {
            // Arrange
            var category = new Category { Id = Guid.NewGuid(), Name = "Vegetables" };

            // Act
            Action act = () => _businessRules.CategoryMustExistWhenRequested(category);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public async Task CategoryNameCannotBeDuplicatedWhenInserted_WhenNameExists_ShouldThrowBusinessException()
        {
            // Arrange
            var categoryName = "Vegetables";
            _mockCategoryRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<Func<IQueryable<Category>, IIncludableQueryable<Category, object>>?>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Category { Id = Guid.NewGuid(), Name = "Vegetables" });

            // Act
            Func<Task> act = async () => await _businessRules.CategoryNameCannotBeDuplicatedWhenInserted(categoryName);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage(CategoryMessages.CategoryAlreadyExistsMessage);
        }

        [Fact]
        public async Task CategoryNameCannotBeDuplicatedWhenInserted_WhenNameDoesNotExist_ShouldNotThrowException()
        {
            // Arrange
            var categoryName = "Vegetables";
            _mockCategoryRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<Func<IQueryable<Category>, IIncludableQueryable<Category, object>>?>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category)null!);

            // Act
            Func<Task> act = async () => await _businessRules.CategoryNameCannotBeDuplicatedWhenInserted(categoryName);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task CategoryCannotBeDeletedIfHasIngredients_WhenHasIngredients_ShouldThrowBusinessException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            _mockIngredientRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Domain.Entities.Ingredient, bool>>>(),
                    It.IsAny<Func<IQueryable<Domain.Entities.Ingredient>, IIncludableQueryable<Domain.Entities.Ingredient, object>>?>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Domain.Entities.Ingredient { Id = Guid.NewGuid(), CategoryId = categoryId, Name = "Milk" });

            // Act
            Func<Task> act = async () => await _businessRules.CategoryCannotBeDeletedIfHasIngredients(categoryId);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage(CategoryMessages.CategoryCannotBeDeletedDueToDependenciesMessage);
        }

        [Fact]
        public async Task CategoryCannotBeDeletedIfHasIngredients_WhenNoIngredients_ShouldNotThrowException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            _mockIngredientRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Domain.Entities.Ingredient, bool>>>(),
                    It.IsAny<Func<IQueryable<Domain.Entities.Ingredient>, IIncludableQueryable<Domain.Entities.Ingredient, object>>?>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Ingredient)null!);

            // Act
            Func<Task> act = async () => await _businessRules.CategoryCannotBeDeletedIfHasIngredients(categoryId);

            // Assert
            await act.Should().NotThrowAsync();
        }
    }
}
