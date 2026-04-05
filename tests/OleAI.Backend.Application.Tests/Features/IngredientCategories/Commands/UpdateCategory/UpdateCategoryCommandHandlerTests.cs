using System.Linq.Expressions;
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
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.IngredientCategories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CategoryBusinessRules _rules;
    private readonly UpdateCategoryCommandHandler _handler;

    public UpdateCategoryCommandHandlerTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockMapper = new Mock<IMapper>();

        _rules = new CategoryBusinessRules(_mockCategoryRepository.Object, _mockIngredientRepository.Object);
        _handler = new UpdateCategoryCommandHandler(_mockCategoryRepository.Object, _mockMapper.Object, _rules);
    }

    [Fact]
    public async Task Handle_WhenCategoryNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new UpdateCategoryCommand
        {
            Request = new UpdateCategoryRequest { Id = Guid.NewGuid(), Name = "Yeni" }
        };

        _mockCategoryRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(CategoryMessages.CategoryNotFoundMessage);
    }

    [Fact]
    public async Task Handle_WhenDuplicateNameExists_ShouldThrowBusinessException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new UpdateCategoryCommand
        {
            Request = new UpdateCategoryRequest { Id = categoryId, Name = "Baharat" }
        };

        _mockCategoryRepository
            .SetupSequence(r => r.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Category(categoryId, "Eski"))
            .ReturnsAsync(new Category(Guid.NewGuid(), "Baharat"));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage(CategoryMessages.CategoryAlreadyExistsMessage);

        _mockCategoryRepository.Verify(r => r.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_ShouldUpdateCategoryAndReturnSuccessMessage()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = new Category(categoryId, "Eski Ad");

        var command = new UpdateCategoryCommand
        {
            Request = new UpdateCategoryRequest { Id = categoryId, Name = "Yeni Ad" }
        };

        _mockCategoryRepository
            .SetupSequence(r => r.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory)
            .ReturnsAsync((Category?)null);

        _mockCategoryRepository
            .Setup(r => r.UpdateAsync(existingCategory, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(CategoryMessages.UpdatedCategoryMessage);

        _mockMapper.Verify(m => m.Map(command.Request, existingCategory), Times.Once);
        _mockCategoryRepository.Verify(r => r.UpdateAsync(existingCategory, It.IsAny<CancellationToken>()), Times.Once);
    }
}
