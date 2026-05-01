using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Commands.ApproveRecipe;
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Commands.ApproveRecipe;

public class ApproveRecipeCommandHandlerTests
{
    private readonly Mock<IRecipeRepository> _mockRecipeRepository;
    private readonly RecipeBusinessRules _rules;
    private readonly ApproveRecipeCommandHandler _handler;

    public ApproveRecipeCommandHandlerTests()
    {
        _mockRecipeRepository = new Mock<IRecipeRepository>();
        _rules = new RecipeBusinessRules(Mock.Of<IIngredientRepository>(), Mock.Of<IRecipeCommentRepository>());
        _handler = new ApproveRecipeCommandHandler(_mockRecipeRepository.Object, _rules);
    }

    [Fact]
    public async Task Handle_WhenValidRecipe_ShouldApproveAndReturnMessage()
    {
        // Arrange
        var recipe = Recipe.Create(Guid.NewGuid(), "Title", "Desc", RecipeVisibility.Public);
        recipe.PassLlmFilter(); // Status becomes PendingAdminApproval

        _mockRecipeRepository.Setup(r => r.GetAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<Recipe, bool>>>(),
                It.IsAny<System.Func<System.Linq.IQueryable<Recipe>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Recipe, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        var command = new ApproveRecipeCommand { Id = recipe.Id };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(RecipeMessages.RecipeApprovedAndPublished);
        recipe.Status.Should().Be(RecipeStatus.Published);
        _mockRecipeRepository.Verify(r => r.UpdateAsync(recipe, It.IsAny<CancellationToken>()), Times.Once);
    }
}
