using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Ingredients.Constants;
using OleAI.Backend.Application.Features.Ingredients.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Ingredients.Rules;

public class IngredientBusinessRulesTests
{
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly IngredientBusinessRules _rules;

    public IngredientBusinessRulesTests()
    {
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _rules = new IngredientBusinessRules(_mockIngredientRepository.Object);
    }

    [Fact]
    public void IngredientMustExistWhenRequested_WhenIngredientIsNull_ShouldThrowNotFoundException()
    {
        // Act
        Action act = () => _rules.IngredientMustExistWhenRequested(null);

        // Assert
        act.Should().Throw<NotFoundException>()
            .WithMessage(IngredientMessages.IngredientNotFoundMessage);
    }

    [Fact]
    public void IngredientMustExistWhenRequested_WhenIngredientExists_ShouldNotThrow()
    {
        // Arrange
        var ingredient = new Ingredient(Guid.NewGuid(), "Domates", Guid.NewGuid(), MeasurementCategory.Count);

        // Act
        Action act = () => _rules.IngredientMustExistWhenRequested(ingredient);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public async Task IngredientNameCannotBeDuplicatedWhenInserted_WhenDuplicateExists_ShouldThrowBusinessException()
    {
        // Arrange
        _mockIngredientRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Ingredient(Guid.NewGuid(), "Domates", Guid.NewGuid(), MeasurementCategory.Count));

        // Act
        Func<Task> act = async () => await _rules.IngredientNameCannotBeDuplicatedWhenInserted("domates");

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage(IngredientMessages.ActiveIngredientAlreadyExistsMessage);
    }

    [Fact]
    public async Task IngredientNameCannotBeDuplicatedWhenInserted_WhenNoDuplicate_ShouldNotThrow()
    {
        // Arrange
        _mockIngredientRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ingredient?)null);

        // Act
        Func<Task> act = async () => await _rules.IngredientNameCannotBeDuplicatedWhenInserted("Domates");

        // Assert
        await act.Should().NotThrowAsync();
    }
}
