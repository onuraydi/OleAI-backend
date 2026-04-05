using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.IngredientCategories.Commands.DeleteCategory;
using OleAI.Backend.Application.Features.IngredientCategories.Constants;
using OleAI.Backend.Application.Features.IngredientCategories.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using System.Linq.Expressions;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;

namespace OleAI.Backend.Application.Tests.Features.IngredientCategories.Commands
{
    public class DeleteCategoryCommandHandlerTests
    {
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<IIngredientRepository> _mockIngredientRepository;
        private readonly CategoryBusinessRules _categoryBusinessRules;
        private readonly DeleteCategoryCommandHandler _handler;

        public DeleteCategoryCommandHandlerTests()
        {
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockIngredientRepository = new Mock<IIngredientRepository>();
            _categoryBusinessRules = new CategoryBusinessRules(_mockCategoryRepository.Object, _mockIngredientRepository.Object);
            
            _handler = new DeleteCategoryCommandHandler(_mockCategoryRepository.Object, _categoryBusinessRules);
        }

        [Fact]
        public async Task Handle_WhenValidRequest_ShouldDeleteCategoryAndReturnMessage()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var command = new DeleteCategoryCommand { Id = categoryId };
            var existingCategory = new Category { Id = categoryId, Name = "Dairy" };

            // Mock Category Must Exist (return existing category)
            _mockCategoryRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<Func<IQueryable<Category>, IIncludableQueryable<Category, object>>?>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCategory);

            // Mock Has Ingredients Check (return null => no ingredients)
            _mockIngredientRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Ingredient, bool>>>(),
                    It.IsAny<Func<IQueryable<Ingredient>, IIncludableQueryable<Ingredient, object>>?>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Ingredient)null!);

            _mockCategoryRepository.Setup(r => r.DeleteAsync(existingCategory, false)).ReturnsAsync(existingCategory);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(CategoryMessages.DeletedCategoryMessage);
            _mockCategoryRepository.Verify(r => r.DeleteAsync(existingCategory, false), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenCategoryNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var command = new DeleteCategoryCommand { Id = Guid.NewGuid() };

            // Mock Category Must Exist (return null)
            _mockCategoryRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category)null!);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage(CategoryMessages.CategoryNotFoundMessage);

            _mockCategoryRepository.Verify(r => r.DeleteAsync(It.IsAny<Category>(), It.IsAny<bool>()), Times.Never);
        }
    }
}
