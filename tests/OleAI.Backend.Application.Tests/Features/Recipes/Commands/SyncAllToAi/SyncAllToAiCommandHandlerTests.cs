using System.Linq.Expressions;
using System.Reflection;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Commands.SyncAllToAi;
using OleAI.Backend.Application.Features.Recipes.Queries.GetRecipesForAi;
using OleAI.Backend.Application.Services.AiRecommendation;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Commands.SyncAllToAi;

public class SyncAllToAiCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldSyncToAi_Empty()
    {
        var mockRepo = new Mock<IRecipeRepository>();
        var mockAiService = new Mock<IAiRecommendationService>();
        var handler = new SyncAllToAiCommandHandler(mockRepo.Object, mockAiService.Object);

        var command = new SyncAllToAiCommand();
        var recipes = new Paginate<Recipe>
        {
            Items = new List<Recipe>(),
            TotalItems = 0
        };

        mockRepo.Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Recipe>>(), It.IsAny<CancellationToken>())).ReturnsAsync(recipes);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Be("0 recipes successfully synced with AI engine.");
    }

    [Fact]
    public async Task Handle_ShouldSyncToAi_WithRecipes()
    {
        var mockRepo = new Mock<IRecipeRepository>();
        var mockAiService = new Mock<IAiRecommendationService>();
        var handler = new SyncAllToAiCommandHandler(mockRepo.Object, mockAiService.Object);

        var command = new SyncAllToAiCommand();
        var recipe = Recipe.Create(Guid.NewGuid(), "T", "D", OleAI.Backend.Domain.Enums.RecipeDifficulty.Easy, 1, 1, 1, OleAI.Backend.Domain.Enums.RecipeVisibility.Public);
        var idProp = typeof(Entity<Guid>).GetProperty("Id");
        idProp.SetValue(recipe, Guid.NewGuid());

        var categoryProp = typeof(Recipe).GetProperty("Status", BindingFlags.Public | BindingFlags.Instance);
        categoryProp.SetValue(recipe, OleAI.Backend.Domain.Enums.RecipeStatus.Published);

        typeof(Recipe).GetProperty("Interactions", BindingFlags.Public | BindingFlags.Instance)?.SetValue(recipe, new List<RecipeInteraction>());

        var recipes = new Paginate<Recipe>
        {
            Items = new List<Recipe> { recipe },
            TotalItems = 1
        };

        mockRepo.Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Recipe>>(), It.IsAny<CancellationToken>())).ReturnsAsync(recipes);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Be("1 recipes successfully synced with AI engine.");
        mockAiService.Verify(a => a.SyncRecipeAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<List<RecipeIngredientAiDto>>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
