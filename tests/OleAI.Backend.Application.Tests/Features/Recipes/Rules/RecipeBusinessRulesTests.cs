using System.Linq.Expressions;
using System.Security.Claims;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Rules;

public class RecipeBusinessRulesTests
{
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<IRecipeCommentRepository> _mockRecipeCommentRepository;
    private readonly RecipeBusinessRules _rules;

    public RecipeBusinessRulesTests()
    {
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockRecipeCommentRepository = new Mock<IRecipeCommentRepository>();
        _rules = new RecipeBusinessRules(_mockIngredientRepository.Object, _mockRecipeCommentRepository.Object);
    }

    [Fact]
    public async Task CheckIfIngredientsExistAsync_WhenIngredientsMissing_ShouldThrowBusinessException()
    {
        // Arrange
        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        _mockIngredientRepository.Setup(r => r.GetCountAsync(
                It.IsAny<Expression<Func<Ingredient, bool>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1); // Only 1 found

        // Act
        Func<Task> act = async () => await _rules.CheckIfIngredientsExistAsync(ids, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>().WithMessage(RecipeMessages.InvalidIngredientsSelectedForRecipe);
    }

    [Fact]
    public void CheckIfRecipeExists_WhenRecipeIsNull_ShouldThrowNotFoundException()
    {
        // Act
        Action act = () => _rules.CheckIfRecipeExists(null);

        // Assert
        act.Should().Throw<NotFoundException>().WithMessage(RecipeMessages.RecipeNotFoundOrDeleted);
    }

    [Fact]
    public void CheckIfUserIsOwner_WhenNotOwner_ShouldThrowAuthorizationException()
    {
        // Arrange
        var recipe = Recipe.Create(Guid.NewGuid(), "Title", "Desc", OleAI.Backend.Domain.Enums.RecipeDifficulty.Medium, 2, 10, 20, RecipeVisibility.Public);
        var otherUserId = Guid.NewGuid();

        // Act
        Action act = () => _rules.CheckIfUserIsOwner(recipe, otherUserId);

        // Assert
        act.Should().Throw<AuthorizationException>().WithMessage(RecipeMessages.UnauthorizedRecipeAccess);
    }

    [Fact]
    public void CheckIfAdmin_WhenNotAdmin_ShouldThrowAuthorizationException()
    {
        // Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity());

        // Act
        Action act = () => _rules.CheckIfAdmin(user);

        // Assert
        act.Should().Throw<AuthorizationException>().WithMessage(RecipeMessages.AdminRoleRequired);
    }

    [Fact]
    public void CheckIfRecipeIsInteractable_WhenPrivateAndNotOwner_ShouldThrowAuthorizationException()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var otherId = Guid.NewGuid();
        var recipe = Recipe.Create(ownerId, "Title", "Desc", OleAI.Backend.Domain.Enums.RecipeDifficulty.Medium, 2, 10, 20, RecipeVisibility.Private);

        // Act
        Action act = () => _rules.CheckIfRecipeIsInteractable(recipe, otherId);

        // Assert
        act.Should().Throw<AuthorizationException>().WithMessage(RecipeMessages.RecipeNotOpenForInteractionYet);
    }

    [Fact]
    public async Task CheckIfParentCommentExistsAsync_WhenCommentBelongsToOtherRecipe_ShouldThrowNotFoundException()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();
        _mockRecipeCommentRepository.Setup(r => r.AnyAsync(
                It.IsAny<Expression<Func<RecipeComment, bool>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _rules.CheckIfParentCommentExistsAsync(parentId, recipeId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage(RecipeMessages.TargetCommentMissing);
    }
}

