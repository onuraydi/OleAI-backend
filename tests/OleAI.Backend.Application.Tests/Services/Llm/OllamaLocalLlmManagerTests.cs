using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using OleAI.Backend.Application.Services.Llm;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Services.Llm;

public class OllamaLocalLlmManagerTests
{
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly IOptions<OllamaSettings> _settings;
    private readonly OllamaLocalLlmManager _service;

    public OllamaLocalLlmManagerTests()
    {
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost:11434") };

        _settings = Options.Create(new OllamaSettings
        {
            BaseUrl = "http://localhost:11434",
            ModelName = "test-model",
            SystemPrompt = "test-system"
        });

        _service = new OllamaLocalLlmManager(_httpClient, _settings, _mockIngredientRepository.Object);
    }

    [Fact]
    public async Task CheckRecipeContentForSpamAsync_WhenClean_ShouldReturnTrue()
    {
        // Arrange
        var responsePayload = new { response = "{\"IsClean\": true, \"Reason\": \"OK\"}" };
        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(responsePayload))
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _service.CheckRecipeContentForSpamAsync("Title", "Desc", CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task AnalyzeIngredientRequestAsync_WhenLlmReturnsValidJson_ShouldReturnOutput()
    {
        // Arrange
        _mockIngredientRepository.Setup(r => r.GetListAsync(It.IsAny<OleAI.Backend.Core.Repositories.Options.QueryOptions<Ingredient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Paginate<Ingredient>());

        var responsePayload = new { response = "{\"IsNew\": true}" };
        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(responsePayload))
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _service.AnalyzeIngredientRequestAsync("NewIng", CancellationToken.None);

        // Assert
        result.Should().Contain("IsNew");
    }
}
