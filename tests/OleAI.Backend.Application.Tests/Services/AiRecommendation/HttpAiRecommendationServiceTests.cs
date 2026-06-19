using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using OleAI.Backend.Application.Services.AiRecommendation;
using Xunit;

namespace OleAI.Backend.Application.Tests.Services.AiRecommendation;

public class HttpAiRecommendationServiceTests
{
    private readonly Mock<ILogger<HttpAiRecommendationService>> _mockLogger;
    private readonly Mock<IOptions<AiServiceSettings>> _mockOptions;
    private readonly AiServiceSettings _settings;

    public HttpAiRecommendationServiceTests()
    {
        _mockLogger = new Mock<ILogger<HttpAiRecommendationService>>();
        _settings = new AiServiceSettings
        {
            BaseUrl = "https://test-ai.oleai.com/",
            ApiKey = "test-api-key"
        };

        _mockOptions = new Mock<IOptions<AiServiceSettings>>();
        _mockOptions.Setup(o => o.Value).Returns(_settings);
    }

    [Fact]
    public async Task GetRecommendationsAsync_WhenSuccessful_ShouldReturnResponse()
    {
        // Arrange
        var expectedResponse = new AiRecommendationResponse
        {
            Recipes = new List<RecommendedRecipeItem>
            {
                new RecommendedRecipeItem { RecipeId = Guid.NewGuid(), Score = 0.9 }
            }
        };

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri!.ToString().EndsWith("/api/v1/recommendations")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(expectedResponse)
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var service = new HttpAiRecommendationService(httpClient, _mockOptions.Object, _mockLogger.Object);
        var payload = new AiRecommendationPayload { UserId = Guid.NewGuid(), Mode = "Default" };

        // Act
        var result = await service.GetRecommendationsAsync(payload, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Recipes.Count, result.Recipes.Count);
    }

    [Fact]
    public async Task GetRecommendationsAsync_WhenApiFails_ShouldCatchExceptionAndReturnEmptyResponse()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var service = new HttpAiRecommendationService(httpClient, _mockOptions.Object, _mockLogger.Object);
        var payload = new AiRecommendationPayload { UserId = Guid.NewGuid(), Mode = "Default" };

        // Act
        var result = await service.GetRecommendationsAsync(payload, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Recipes);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SyncRecipeAsync_WhenSuccessful_ShouldLogInformation()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri!.ToString().EndsWith("/api/v1/recipes/sync")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var service = new HttpAiRecommendationService(httpClient, _mockOptions.Object, _mockLogger.Object);

        // Act
        await service.SyncRecipeAsync(Guid.NewGuid(), "Test Recipe", new List<RecipeIngredientAiDto>(), "Dessert", new List<string> { "sweet" }, true, 10, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("synced successfully")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SyncRecipeAsync_WhenApiFails_ShouldCatchExceptionAndLogWarning()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        var httpClient = new HttpClient(handlerMock.Object);
        var service = new HttpAiRecommendationService(httpClient, _mockOptions.Object, _mockLogger.Object);

        // Act
        await service.SyncRecipeAsync(Guid.NewGuid(), "Test Recipe", new List<RecipeIngredientAiDto>(), "Dessert", new List<string> { "sweet" }, true, 10, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed to sync recipe")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
