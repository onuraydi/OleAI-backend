using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.IngredientCategories.Commands.CreateCategory;
using OleAI.Backend.Application.Features.IngredientCategories.Constants;
using OleAI.Backend.Application.Features.IngredientCategories.Dtos.Requests;
using OleAI.Backend.Application.Features.IngredientCategories.Dtos.Response;
using OleAI.Backend.Application.Features.IngredientCategories.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using System.Linq.Expressions;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;

namespace OleAI.Backend.Application.Tests.Features.IngredientCategories.Commands
{
    public class CreateCategoryCommandHandlerTests
    {
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<IIngredientRepository> _mockIngredientRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CategoryBusinessRules _categoryBusinessRules;
        private readonly CreateCategoryCommandHandler _handler;

        public CreateCategoryCommandHandlerTests()
        {
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockIngredientRepository = new Mock<IIngredientRepository>();
            _mockMapper = new Mock<IMapper>();
            _categoryBusinessRules = new CategoryBusinessRules(_mockCategoryRepository.Object, _mockIngredientRepository.Object);
            
            _handler = new CreateCategoryCommandHandler(_mockCategoryRepository.Object, _mockMapper.Object, _categoryBusinessRules);
        }

        [Fact]
        public async Task Handle_WhenValidRequest_ShouldCreateCategoryAndReturnResponse()
        {
            // Arrange
            var request = new CreateCategoryRequest { Name = "Dairy" };
            var command = new CreateCategoryCommand { Request = request };

            var mappedCategory = new Category { Name = "Dairy" };
            var createdCategory = new Category { Id = Guid.NewGuid(), Name = "Dairy" };
            
            var responseDto = new CreatedCategoryResponse { Id = createdCategory.Id, Name = "Dairy" };

            // Mock business rule: No overlapping name exists
            _mockCategoryRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<Func<IQueryable<Category>, IIncludableQueryable<Category, object>>?>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category)null!);

            _mockMapper.Setup(m => m.Map<Category>(request)).Returns(mappedCategory);
            _mockCategoryRepository.Setup(r => r.AddAsync(mappedCategory)).ReturnsAsync(createdCategory);
            _mockMapper.Setup(m => m.Map<CreatedCategoryResponse>(createdCategory)).Returns(responseDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Dairy");
            result.Message.Should().Be(CategoryMessages.CreatedCategoryMessage);

            _mockCategoryRepository.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Once);
            _mockMapper.Verify(m => m.Map<Category>(request), Times.Once);
            _mockMapper.Verify(m => m.Map<CreatedCategoryResponse>(createdCategory), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenCategoryNameAlreadyExists_ShouldThrowBusinessException()
        {
            // Arrange
            var request = new CreateCategoryRequest { Name = "Dairy" };
            var command = new CreateCategoryCommand { Request = request };
            var existingCategory = new Category { Id = Guid.NewGuid(), Name = "Dairy" };

            // Mock business rule: Name already exists
            _mockCategoryRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<Func<IQueryable<Category>, IIncludableQueryable<Category, object>>?>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCategory);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage(CategoryMessages.CategoryAlreadyExistsMessage);

            _mockCategoryRepository.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Never);
        }
    }
}
