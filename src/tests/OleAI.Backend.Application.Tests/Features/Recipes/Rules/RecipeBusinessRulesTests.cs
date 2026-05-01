using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Rules;

public class RecipeBusinessRulesTests
{
    private readonly Mock<IIngredientRepository> _mockIngredientRepository = new();
    private readonly Mock<IRecipeCommentRepository> _mockRecipeCommentRepository = new();
    private readonly RecipeBusinessRules _rules;

    public RecipeBusinessRulesTests()
    {
        _rules = new RecipeBusinessRules(_mockIngredientRepository.Object, _mockRecipeCommentRepository.Object);
    }

    [Fact]
    public async Task CheckIfIngredientsExistAsync_WhenIngredientListIsEmpty_ShouldNotQueryRepository()
    {
        await _rules.CheckIfIngredientsExistAsync([], CancellationToken.None);

        _mockIngredientRepository.Verify(r => r.GetCountAsync(
            It.IsAny<Expression<Func<Ingredient, bool>>>(),
            false,
            true,
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CheckIfIngredientsExistAsync_WhenSomeIngredientsDoNotExist_ShouldThrowBusinessException()
    {
        _mockIngredientRepository
            .Setup(r => r.GetCountAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        Func<Task> act = () => _rules.CheckIfIngredientsExistAsync([Guid.NewGuid(), Guid.NewGuid()], CancellationToken.None);

        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage(RecipeMessages.InvalidIngredientsSelectedForRecipe);
    }

    [Fact]
    public async Task CheckIfIngredientsExistAsync_WhenAllIngredientsExist_ShouldNotThrow()
    {
        var ids = [Guid.NewGuid(), Guid.NewGuid()];

        _mockIngredientRepository
            .Setup(r => r.GetCountAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ids.Count);

        Func<Task> act = () => _rules.CheckIfIngredientsExistAsync(ids, CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public void CheckIfRecipeExists_WhenRecipeIsNull_ShouldThrowNotFoundException()
    {
        Action act = () => _rules.CheckIfRecipeExists(null);

        act.Should().Throw<NotFoundException>()
            .WithMessage(RecipeMessages.RecipeNotFoundOrDeleted);
    }

    [Fact]
    public void CheckIfRecipeExists_WhenRecipeIsNotNull_ShouldNotThrow()
    {
        var recipe = Recipe.Create(Guid.NewGuid(), "Test", "Desc", RecipeVisibility.Private);

        Action act = () => _rules.CheckIfRecipeExists(recipe);

        act.Should().NotThrow();
    }

    [Fact]
    public void CheckIfUserIsOwner_WhenUserIsNotOwner_ShouldThrowAuthorizationException()
    {
        var recipe = Recipe.Create(Guid.NewGuid(), "Test", "Desc", RecipeVisibility.Private);

        Action act = () => _rules.CheckIfUserIsOwner(recipe, Guid.NewGuid());

        act.Should().Throw<AuthorizationException>()
            .WithMessage(RecipeMessages.UnauthorizedRecipeAccess);
    }

    [Fact]
    public void CheckIfUserIsOwner_WhenUserIsOwner_ShouldNotThrow()
    {
        var userId = Guid.NewGuid();
        var recipe = Recipe.Create(userId, "Test", "Desc", RecipeVisibility.Private);

        Action act = () => _rules.CheckIfUserIsOwner(recipe, userId);

        act.Should().NotThrow();
    }

    [Fact]
    public void CheckIfAdmin_WhenUserIsNotAdmin_ShouldThrowAuthorizationException()
    {
        Action act = () => _rules.CheckIfAdmin(new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity()));

        act.Should().Throw<AuthorizationException>()
            .WithMessage(RecipeMessages.AdminRoleRequired);
    }

    [Fact]
    public void CheckIfAdmin_WhenUserIsAdmin_ShouldNotThrow()
    {
        var user = new System.Security.Claims.ClaimsPrincipal(
            new System.Security.Claims.ClaimsIdentity(
                [new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "Admin")],
                "TestAuth"));

        Action act = () => _rules.CheckIfAdmin(user);

        act.Should().NotThrow();
    }

    [Fact]
    public void CheckIfRecipeIsInteractable_WhenRecipeIsPublicAndPublished_ShouldNotThrow()
    {
        var recipe = Recipe.Create(Guid.NewGuid(), "Test", "Desc", RecipeVisibility.Public);
        recipe.PassLlmFilter();
        typeof(Recipe).GetProperty(nameof(Recipe.Status))!.SetValue(recipe, RecipeStatus.Published);

        Action act = () => _rules.CheckIfRecipeIsInteractable(recipe, Guid.NewGuid());

        act.Should().NotThrow();
    }

    [Fact]
    public void CheckIfRecipeIsInteractable_WhenRecipeIsPrivateAndUserIsNotOwner_ShouldThrowAuthorizationException()
    {
        var recipe = Recipe.Create(Guid.NewGuid(), "Test", "Desc", RecipeVisibility.Private);

        Action act = () => _rules.CheckIfRecipeIsInteractable(recipe, Guid.NewGuid());

        act.Should().Throw<AuthorizationException>()
            .WithMessage(RecipeMessages.RecipeNotOpenForInteractionYet);
    }

    [Fact]
    public async Task CheckIfParentCommentExistsAsync_WhenParentCommentDoesNotExist_ShouldThrowNotFoundException()
    {
        var parentCommentId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();

        _mockRecipeCommentRepository
            .Setup(r => r.AnyAsync(It.IsAny<Expression<Func<RecipeComment, bool>>>(), false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Func<Task> act = () => _rules.CheckIfParentCommentExistsAsync(parentCommentId, recipeId, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(RecipeMessages.TargetCommentMissing);
    }

    [Fact]
    public async Task CheckIfParentCommentExistsAsync_WhenParentCommentExists_ShouldNotThrow()
    {
        var parentCommentId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();

        _mockRecipeCommentRepository
            .Setup(r => r.AnyAsync(It.IsAny<Expression<Func<RecipeComment, bool>>>(), false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        Func<Task> act = () => _rules.CheckIfParentCommentExistsAsync(parentCommentId, recipeId, CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public void CheckIfUserIsCommentOwner_WhenCommentBelongsToAnotherUser_ShouldThrowAuthorizationException()
    {
        var comment = new RecipeComment(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "yorum");

        Action act = () => _rules.CheckIfUserIsCommentOwner(comment, Guid.NewGuid());

        act.Should().Throw<AuthorizationException>()
            .WithMessage(RecipeMessages.UnauthorizedCommentAction);
    }

    [Fact]
    public void CheckIfUserIsCommentOwner_WhenCommentBelongsToCurrentUser_ShouldNotThrow()
    {
        var userId = Guid.NewGuid();
        var comment = new RecipeComment(Guid.NewGuid(), Guid.NewGuid(), userId, "yorum");

        Action act = () => _rules.CheckIfUserIsCommentOwner(comment, userId);

        act.Should().NotThrow();
    }
}
