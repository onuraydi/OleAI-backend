using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.IngredientCategories.Commands.DeleteCategory;
using OleAI.Backend.Application.Features.IngredientCategories.Constants;
using OleAI.Backend.Application.Features.IngredientCategories.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.IngredientCategories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly CategoryBusinessRules _rules;
    private readonly DeleteCategoryCommandHandler _handler;

    public DeleteCategoryCommandHandlerTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockIngredientRepository = new Mock<IIngredientRepository>();

        _rules = new CategoryBusinessRules(_mockCategoryRepository.Object, _mockIngredientRepository.Object);
        _handler = new DeleteCategoryCommandHandler(_mockCategoryRepository.Object, _rules);
    }

    [Fact]
    public async Task Handle_WhenCategoryNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new DeleteCategoryCommand { Id = Guid.NewGuid() };

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
    public async Task Handle_WhenCategoryHasDependentIngredients_ShouldThrowBusinessException()
    {
        // Arrange
        var category = new Category(Guid.NewGuid(), "Bakliyat");
        var command = new DeleteCategoryCommand { Id = category.Id };

        _mockCategoryRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _mockIngredientRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Ingredient(Guid.NewGuid(), "Mercimek", category.Id, "kg"));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage(CategoryMessages.CategoryCannotBeDeletedDueToDependenciesMessage);

        _mockCategoryRepository.Verify(r => r.DeleteAsync(It.IsAny<Category>(), false, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCategoryCanBeDeleted_ShouldDeleteAndReturnSuccessMessage()
    {
        // Arrange
        var category = new Category(Guid.NewGuid(), "Baharat");
        var command = new DeleteCategoryCommand { Id = category.Id };

        _mockCategoryRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _mockIngredientRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ingredient?)null);

        _mockCategoryRepository
            .Setup(r => r.DeleteAsync(category, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(CategoryMessages.DeletedCategoryMessage);
        _mockCategoryRepository.Verify(r => r.DeleteAsync(category, false, It.IsAny<CancellationToken>()), Times.Once);
    }
}
