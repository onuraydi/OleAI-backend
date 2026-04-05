using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.IngredientCategories.Constants;
using OleAI.Backend.Application.Features.IngredientCategories.Dtos.Response;
using OleAI.Backend.Application.Features.IngredientCategories.Queries.GetCategoryById;
using OleAI.Backend.Application.Features.IngredientCategories.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using System.Linq.Expressions;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;

namespace OleAI.Backend.Application.Tests.Features.IngredientCategories.Queries
{
    public class GetCategoryByIdQueryHandlerTests
    {
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<IIngredientRepository> _mockIngredientRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CategoryBusinessRules _categoryBusinessRules;
        private readonly GetCategoryByIdQueryHandler _handler;

        public GetCategoryByIdQueryHandlerTests()
        {
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockIngredientRepository = new Mock<IIngredientRepository>();
            _mockMapper = new Mock<IMapper>();
            _categoryBusinessRules = new CategoryBusinessRules(_mockCategoryRepository.Object, _mockIngredientRepository.Object);
            
            _handler = new GetCategoryByIdQueryHandler(_mockCategoryRepository.Object, _mockMapper.Object, _categoryBusinessRules);
        }

        [Fact]
        public async Task Handle_WhenCategoryExists_ShouldReturnCategoryResponse()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var query = new GetCategoryByIdQuery { Id = categoryId };
            var existingCategory = new Category { Id = categoryId, Name = "Meat" };
            var expectedResponse = new CategoryResponse { Id = categoryId, Name = "Meat" };

            _mockCategoryRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCategory);

            _mockMapper.Setup(m => m.Map<CategoryResponse>(existingCategory))
                .Returns(expectedResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(categoryId);
            result.Name.Should().Be("Meat");

            _mockCategoryRepository.Verify(r => r.GetAsync(
                It.IsAny<Expression<Func<Category, bool>>>(),
                null, false, true, It.IsAny<CancellationToken>()), Times.Once);
            _mockMapper.Verify(m => m.Map<CategoryResponse>(existingCategory), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenCategoryDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var query = new GetCategoryByIdQuery { Id = Guid.NewGuid() };

            _mockCategoryRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category)null!);

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage(CategoryMessages.CategoryNotFoundMessage);
        }
    }
}
