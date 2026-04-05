using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.IngredientCategories.Commands.UpdateCategory;
using OleAI.Backend.Application.Features.IngredientCategories.Constants;
using OleAI.Backend.Application.Features.IngredientCategories.Dtos.Requests;
using OleAI.Backend.Application.Features.IngredientCategories.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using System.Linq.Expressions;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;

namespace OleAI.Backend.Application.Tests.Features.IngredientCategories.Commands
{
    public class UpdateCategoryCommandHandlerTests
    {
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<IIngredientRepository> _mockIngredientRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CategoryBusinessRules _categoryBusinessRules;
        private readonly UpdateCategoryCommandHandler _handler;

        public UpdateCategoryCommandHandlerTests()
        {
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockIngredientRepository = new Mock<IIngredientRepository>();
            _mockMapper = new Mock<IMapper>();
            _categoryBusinessRules = new CategoryBusinessRules(_mockCategoryRepository.Object, _mockIngredientRepository.Object);
            
            _handler = new UpdateCategoryCommandHandler(_mockCategoryRepository.Object, _mockMapper.Object, _categoryBusinessRules);
        }

        [Fact]
        public async Task Handle_WhenValidRequest_ShouldUpdateCategoryAndReturnMessage()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var request = new UpdateCategoryRequest { Id = categoryId, Name = "Updated Dairy" };
            var command = new UpdateCategoryCommand { Request = request };
            var existingCategory = new Category { Id = categoryId, Name = "Dairy" };

            // Mock GetAsync to return the existing category
            _mockCategoryRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<Func<IQueryable<Category>, IIncludableQueryable<Category, object>>?>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCategory);

            // In the business rule, mock GetAsync for the duplicate name check to return null (no duplicate)
            _mockCategoryRepository
                .Setup(r => r.GetAsync(
                    It.Is<Expression<Func<Category, bool>>>(expr => expr.Compile().Invoke(new Category { Name = "Updated Dairy" })),
                    It.IsAny<Func<IQueryable<Category>, IIncludableQueryable<Category, object>>?>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category)null!);

            _mockCategoryRepository.Setup(r => r.UpdateAsync(existingCategory)).ReturnsAsync(existingCategory);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(CategoryMessages.UpdatedCategoryMessage);
            _mockMapper.Verify(m => m.Map(request, existingCategory), Times.Once);
            _mockCategoryRepository.Verify(r => r.UpdateAsync(existingCategory), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenCategoryNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var command = new UpdateCategoryCommand { Request = new UpdateCategoryRequest { Id = Guid.NewGuid(), Name = "Dairy" } };

            // Mock GetAsync to return null
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

            _mockCategoryRepository.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Never);
        }
    }
}
