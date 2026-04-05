using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.IngredientCategories.Constants;
using OleAI.Backend.Application.Features.IngredientCategories.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.IngredientCategories.Rules;

public class CategoryBusinessRulesTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly CategoryBusinessRules _rules;

    public CategoryBusinessRulesTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _rules = new CategoryBusinessRules(_mockCategoryRepository.Object, _mockIngredientRepository.Object);
    }

    [Fact]
    public void CategoryMustExistWhenRequested_WhenCategoryIsNull_ShouldThrowNotFoundException()
    {
        // Act
        Action act = () => _rules.CategoryMustExistWhenRequested(null);

        // Assert
        act.Should().Throw<NotFoundException>()
            .WithMessage(CategoryMessages.CategoryNotFoundMessage);
    }

    [Fact]
    public void CategoryMustExistWhenRequested_WhenCategoryExists_ShouldNotThrow()
    {
        // Arrange
        var category = new Category(Guid.NewGuid(), "Bakliyat");

        // Act
        Action act = () => _rules.CategoryMustExistWhenRequested(category);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public async Task CategoryNameCannotBeDuplicatedWhenInserted_WhenDuplicateExists_ShouldThrowBusinessException()
    {
        // Arrange
        _mockCategoryRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Category(Guid.NewGuid(), "Bakliyat"));

        // Act
        Func<Task> act = async () => await _rules.CategoryNameCannotBeDuplicatedWhenInserted("Bakliyat");

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage(CategoryMessages.CategoryAlreadyExistsMessage);
    }

    [Fact]
    public async Task CategoryNameCannotBeDuplicatedWhenInserted_WhenNoDuplicate_ShouldNotThrow()
    {
        // Arrange
        _mockCategoryRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        Func<Task> act = async () => await _rules.CategoryNameCannotBeDuplicatedWhenInserted("Bakliyat");

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task CategoryCannotBeDeletedIfHasIngredients_WhenDependentIngredientExists_ShouldThrowBusinessException()
    {
        // Arrange
        _mockIngredientRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Ingredient(Guid.NewGuid(), "Mercimek", Guid.NewGuid(), "kg"));

        // Act
        Func<Task> act = async () => await _rules.CategoryCannotBeDeletedIfHasIngredients(Guid.NewGuid());

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage(CategoryMessages.CategoryCannotBeDeletedDueToDependenciesMessage);
    }

    [Fact]
    public async Task CategoryCannotBeDeletedIfHasIngredients_WhenNoDependency_ShouldNotThrow()
    {
        // Arrange
        _mockIngredientRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ingredient?)null);

        // Act
        Func<Task> act = async () => await _rules.CategoryCannotBeDeletedIfHasIngredients(Guid.NewGuid());

        // Assert
        await act.Should().NotThrowAsync();
    }
}
