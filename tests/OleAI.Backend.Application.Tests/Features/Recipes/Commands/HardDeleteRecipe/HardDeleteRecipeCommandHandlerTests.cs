using System.Linq.Expressions;
using System.Reflection;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Commands.HardDeleteRecipe;
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Services.ImageStorage;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Commands.HardDeleteRecipe;

public class HardDeleteRecipeCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldDeleteRecipeAndImages()
    {
        var mockRepo = new Mock<IRecipeRepository>();
        var mockImageService = new Mock<IImageService>();
        var handler = new HardDeleteRecipeCommandHandler(mockRepo.Object, mockImageService.Object);

        var id = Guid.NewGuid();
        var command = new HardDeleteRecipeCommand { Id = id };
        var recipe = Recipe.Create(id, "T", "D", OleAI.Backend.Domain.Enums.RecipeDifficulty.Easy, 1, 1, 1, OleAI.Backend.Domain.Enums.RecipeVisibility.Public);

        var img = new RecipeImage { RecipeId = id, PublicId = "public-id", ImageUrl = "url" };
        var imagesField = typeof(Recipe).GetField("_recipeImages", BindingFlags.NonPublic | BindingFlags.Instance);
        var imagesList = (List<RecipeImage>)imagesField.GetValue(recipe);
        imagesList.Add(img);

        mockRepo.Setup(r => r.GetAsync(
            It.IsAny<Expression<Func<Recipe, bool>>>(),
            It.IsAny<Func<IQueryable<Recipe>, IIncludableQueryable<Recipe, object>>>(),
            true, true, It.IsAny<CancellationToken>())).ReturnsAsync(recipe);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Be(RecipeMessages.RecipeHardDeleted);
        mockImageService.Verify(i => i.DeleteAsync("public-id", It.IsAny<CancellationToken>()), Times.Once);
        mockRepo.Verify(r => r.DeleteAsync(recipe, true, It.IsAny<CancellationToken>()), Times.Once);
    }
}
