using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using OleAI.Backend.Application.Services.Llm;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Services.Llm;

public class OllamaLocalLlmManagerTests
{
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<IOptions<OllamaSettings>> _mockOptions;
    private readonly OllamaSettings _settings;

    public OllamaLocalLlmManagerTests()
    {
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _settings = new OllamaSettings
        {
            BaseUrl = "http://localhost:11434",
            ModelName = "llama3"
        };
        _mockOptions = new Mock<IOptions<OllamaSettings>>();
        _mockOptions.Setup(o => o.Value).Returns(_settings);
    }

    private HttpClient CreateMockHttpClient(HttpStatusCode statusCode, string responseContent)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(responseContent)
            });

        return new HttpClient(handlerMock.Object);
    }

    [Fact]
    public async Task AnalyzeIngredientRequestAsync_ShouldReturnJsonFromLlm()
    {
        // Arrange
        var paginatedResult = new Paginate<Ingredient>
        {
            Items = new List<Ingredient> { new Ingredient(Guid.NewGuid(), "Domates", Guid.NewGuid()) }
        };

        _mockIngredientRepository.Setup(r => r.GetListAsync(
            It.IsAny<QueryOptions<Ingredient>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedResult);

        var expectedLlmOutput = "{\"IsNew\": false, \"SuggestedAliasFor\": \"Domates\", \"EstimatedShelfLifeInDays\": 5}";
        var apiResponse = JsonSerializer.Serialize(new { response = expectedLlmOutput });
        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, apiResponse);

        var manager = new OllamaLocalLlmManager(httpClient, _mockOptions.Object, _mockIngredientRepository.Object);

        // Act
        var result = await manager.AnalyzeIngredientRequestAsync("Domaset", CancellationToken.None);

        // Assert
        Assert.Contains("SuggestedAliasFor", result);
        Assert.Contains("Domates", result);
    }

    [Fact]
    public async Task AnalyzeIngredientRequestAsync_WhenLlmFails_ShouldReturnErrorJson()
    {
        // Arrange
        _mockIngredientRepository.Setup(r => r.GetListAsync(
            It.IsAny<QueryOptions<Ingredient>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Paginate<Ingredient> { Items = new List<Ingredient>() });

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Timeout"));

        var httpClient = new HttpClient(handlerMock.Object);
        var manager = new OllamaLocalLlmManager(httpClient, _mockOptions.Object, _mockIngredientRepository.Object);

        // Act
        var result = await manager.AnalyzeIngredientRequestAsync("Elma", CancellationToken.None);

        // Assert
        Assert.Contains("Timeout", result);
    }

    [Fact]
    public async Task CheckRecipeContentForSpamAsync_WhenClean_ShouldReturnTrue()
    {
        // Arrange
        var expectedLlmOutput = "{\"IsClean\": true, \"Reason\": \"Looks good\"}";
        var apiResponse = JsonSerializer.Serialize(new { response = expectedLlmOutput });
        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, apiResponse);
        var manager = new OllamaLocalLlmManager(httpClient, _mockOptions.Object, _mockIngredientRepository.Object);

        // Act
        var result = await manager.CheckRecipeContentForSpamAsync("Kek", "Güzel bir kek tarifi", CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CheckRecipeContentForSpamAsync_WhenSpam_ShouldReturnFalse()
    {
        // Arrange
        var expectedLlmOutput = "{\"IsClean\": false, \"Reason\": \"Spam link found\"}";
        var apiResponse = JsonSerializer.Serialize(new { response = expectedLlmOutput });
        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, apiResponse);
        var manager = new OllamaLocalLlmManager(httpClient, _mockOptions.Object, _mockIngredientRepository.Object);

        // Act
        var result = await manager.CheckRecipeContentForSpamAsync("Kek", "asdfghj", CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CheckRecipeContentForSpamAsync_WhenApiFails_ShouldFallbackToTrue()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Server down"));

        var httpClient = new HttpClient(handlerMock.Object);
        var manager = new OllamaLocalLlmManager(httpClient, _mockOptions.Object, _mockIngredientRepository.Object);

        // Act
        var result = await manager.CheckRecipeContentForSpamAsync("Kek", "Test", CancellationToken.None);

        // Assert
        Assert.True(result); // Fallback is true to not block users when AI is down
    }

    [Fact]
    public async Task AnalyzeRecipeContentAsync_WhenApproved_ShouldCorrectAndReturnJson()
    {
        // Arrange
        var phase1Output = "{\"isApproved\": true, \"isToxic\": false, \"correctedDescription\": \"Corrected desc\", \"rejectionReason\": null}";
        var stepOutput = "{\"reasoning\": \"ok\", \"correctedStep\": \"Corrected step 1\"}";

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new { response = phase1Output }))
            })
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new { response = stepOutput }))
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var manager = new OllamaLocalLlmManager(httpClient, _mockOptions.Object, _mockIngredientRepository.Object);

        // Act
        var result = await manager.AnalyzeRecipeContentAsync("Desc", new List<string> { "Step 1" }, CancellationToken.None);

        // Assert
        Assert.Contains("Corrected desc", result);
        Assert.Contains("Corrected step 1", result);
        Assert.Contains("\"isApproved\":true", result.Replace(" ", ""));
    }

    [Fact]
    public async Task AnalyzeRecipeContentAsync_WhenRejected_ShouldReturnRejectionJson()
    {
        // Arrange
        var phase1Output = "{\"isApproved\": false, \"isToxic\": true, \"correctedDescription\": null, \"rejectionReason\": \"Bad words\"}";

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new { response = phase1Output }))
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var manager = new OllamaLocalLlmManager(httpClient, _mockOptions.Object, _mockIngredientRepository.Object);

        // Act
        var result = await manager.AnalyzeRecipeContentAsync("Bad stuff", new List<string> { "Step 1" }, CancellationToken.None);

        // Assert
        Assert.Contains("\"isApproved\":false", result.Replace(" ", ""));
        Assert.Contains("Bad words", result);
    }

    [Fact]
    public async Task AnalyzeRecipeContentAsync_WhenApiFails_ShouldReturnFallbackJson()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new Exception("Network fail"));

        var httpClient = new HttpClient(handlerMock.Object);
        var manager = new OllamaLocalLlmManager(httpClient, _mockOptions.Object, _mockIngredientRepository.Object);

        // Act
        var result = await manager.AnalyzeRecipeContentAsync("Desc", new List<string> { "Step 1" }, CancellationToken.None);

        // Assert
        Assert.Contains("\"isApproved\":false", result.Replace(" ", ""));
        Assert.Contains("Network fail", result);
    }
}
