using OleAI.Backend.Application.Services.AiRecommendation;
using Xunit;

namespace OleAI.Backend.Application.Tests.Services.AiRecommendation;

public class DummyAiRecommendationServiceTests
{
    [Fact]
    public async Task GetRecommendationsAsync_ShouldReturnEmptyResponse()
    {
        // Arrange
        var service = new DummyAiRecommendationService();
        var payload = new AiRecommendationPayload { UserId = Guid.NewGuid(), Mode = "Default" };

        // Act
        var result = await service.GetRecommendationsAsync(payload, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Recipes);
        Assert.Empty(result.Recipes);
    }

    [Fact]
    public async Task SyncRecipeAsync_ShouldReturnCompletedTask()
    {
        // Arrange
        var service = new DummyAiRecommendationService();

        // Act
        var task = service.SyncRecipeAsync(Guid.NewGuid(), "Title", new List<RecipeIngredientAiDto>());

        // Assert
        Assert.True(task.IsCompletedSuccessfully);
        await task;
    }
}
