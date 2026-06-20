using System.Linq.Expressions;
using System.Reflection;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Queries.GetRecipesForAi;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Queries.GetRecipesForAi;

public class GetRecipesForAiQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnRecipesForAi()
    {
        var mockRepo = new Mock<IRecipeRepository>();
        var handler = new GetRecipesForAiQueryHandler(mockRepo.Object);

        var recipeId = Guid.NewGuid();
        var recipe = Recipe.Create(Guid.NewGuid(), "T", "D", OleAI.Backend.Domain.Enums.RecipeDifficulty.Easy, 1, 1, 1, OleAI.Backend.Domain.Enums.RecipeVisibility.Public);
        var idProp = typeof(Entity<Guid>).GetProperty("Id");
        idProp.SetValue(recipe, recipeId);

        var categoryProp = typeof(Recipe).GetProperty("Status", BindingFlags.Public | BindingFlags.Instance);
        categoryProp.SetValue(recipe, OleAI.Backend.Domain.Enums.RecipeStatus.Published);

        typeof(Recipe).GetProperty("Interactions", BindingFlags.Public | BindingFlags.Instance)?.SetValue(recipe, new List<RecipeInteraction>());

        var recipes = new Paginate<Recipe> { Items = new List<Recipe> { recipe } };

        mockRepo.Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Recipe>>(), It.IsAny<CancellationToken>())).ReturnsAsync(recipes);

        var result = await handler.Handle(new GetRecipesForAiQuery(), CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Title.Should().Be("T");
    }
}
