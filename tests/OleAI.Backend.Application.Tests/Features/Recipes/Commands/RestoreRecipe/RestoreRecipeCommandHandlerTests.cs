using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Commands.RestoreRecipe;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Commands.RestoreRecipe;

public class RestoreRecipeCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldRestoreRecipe()
    {
        var mockRepo = new Mock<IRecipeRepository>();

        var handler = new RestoreRecipeCommandHandler(mockRepo.Object);

        var id = Guid.NewGuid();
        var command = new RestoreRecipeCommand { Id = id };
        var recipe = Recipe.Create(id, "T", "D", RecipeDifficulty.Easy, 1, 1, 1, RecipeVisibility.Public);
        recipe.DeletedDate = DateTime.UtcNow;

        mockRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Recipe, bool>>>(), null, true, true, It.IsAny<CancellationToken>())).ReturnsAsync(recipe);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        recipe.DeletedDate.Should().BeNull();
        mockRepo.Verify(r => r.UpdateAsync(recipe, It.IsAny<CancellationToken>()), Times.Once);
    }
}
