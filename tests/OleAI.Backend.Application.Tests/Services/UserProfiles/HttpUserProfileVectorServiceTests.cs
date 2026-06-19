using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OleAI.Backend.Application.Services.AiRecommendation;
using OleAI.Backend.Application.Services.UserProfiles;

namespace OleAI.Backend.Application.Tests.Services.UserProfiles;

public class HttpUserProfileVectorServiceTests
{
    private readonly Mock<ILogger<HttpUserProfileVectorService>> _mockLogger;
    private readonly Mock<IOptions<AiServiceSettings>> _mockOptions;
    private readonly AiServiceSettings _settings;
    private readonly HttpClient _httpClient;

    public HttpUserProfileVectorServiceTests()
    {
        _mockLogger = new Mock<ILogger<HttpUserProfileVectorService>>();
        _settings = new AiServiceSettings
        {
            BaseUrl = "https://test-ai.oleai.com/",
            ApiKey = "test-api-key"
        };

        _mockOptions = new Mock<IOptions<AiServiceSettings>>();
        _mockOptions.Setup(o => o.Value).Returns(_settings);

        // Mock HttpClient with dummy handler
        var handler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(handler.Object);
    }

    [Fact]
    public void Constructor_Should_Configure_HttpClient_Headers_And_BaseAddress()
    {
        // Act
        var service = new HttpUserProfileVectorService(_httpClient, _mockOptions.Object, _mockLogger.Object);

        // Assert
        Assert.Equal(new Uri(_settings.BaseUrl), _httpClient.BaseAddress);
        Assert.True(_httpClient.DefaultRequestHeaders.Contains("X-API-Key"));
        var apiKeyHeader = _httpClient.DefaultRequestHeaders.GetValues("X-API-Key").First();
        Assert.Equal(_settings.ApiKey, apiKeyHeader);
    }

    [Fact]
    public async Task UpdateProfileVectorAsync_Should_Log_Debug_Message()
    {
        // Arrange
        var service = new HttpUserProfileVectorService(_httpClient, _mockOptions.Object, _mockLogger.Object);
        var userId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();
        var interactionType = "view";

        // Act
        await service.UpdateProfileVectorAsync(userId, recipeId, interactionType, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"User {userId} interacted with {recipeId}")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateDietaryProfileAsync_Should_Return_CompletedTask()
    {
        // Arrange
        var service = new HttpUserProfileVectorService(_httpClient, _mockOptions.Object, _mockLogger.Object);

        // Act
        var task = service.UpdateDietaryProfileAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.True(task.IsCompletedSuccessfully);
        await task;
    }
}
