using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using OleAI.Backend.Application.Services.Llm;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Services.Llm;

public class OllamaLocalLlmManagerTests
{
    [Fact]
    public async Task AnalyzeIngredientRequestAsync_WhenResponseIsValid_ShouldReturnLlmJson()
    {
        // Arrange
        var handler = new StubHttpMessageHandler(_ =>
        {
            var payload = "{\"response\":\"{\\\"IsNew\\\":true}\"}";
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            };
        });

        var httpClient = new HttpClient(handler);
        var settings = Options.Create(new OllamaSettings
        {
            BaseUrl = "http://localhost:11434",
            ModelName = "model",
            SystemPrompt = "prompt"
        });

        var ingredientRepo = new Mock<IIngredientRepository>();
        ingredientRepo
            .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Ingredient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Paginate<Ingredient> { Items = new List<Ingredient>() });

        var sut = new OllamaLocalLlmManager(httpClient, settings, ingredientRepo.Object);

        // Act
        var result = await sut.AnalyzeIngredientRequestAsync("Domat", CancellationToken.None);

        // Assert
        result.Should().Be("{\"IsNew\":true}");
    }

    [Fact]
    public async Task AnalyzeIngredientRequestAsync_WhenLlmReturnsInvalidJson_ShouldReturnFallbackErrorJson()
    {
        // Arrange
        var handler = new StubHttpMessageHandler(_ =>
        {
            var payload = "{\"response\":\"not-a-json\"}";
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            };
        });

        var httpClient = new HttpClient(handler);
        var settings = Options.Create(new OllamaSettings
        {
            BaseUrl = "http://localhost:11434",
            ModelName = "model",
            SystemPrompt = "prompt"
        });

        var ingredientRepo = new Mock<IIngredientRepository>();
        ingredientRepo
            .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Ingredient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Paginate<Ingredient> { Items = new List<Ingredient>() });

        var sut = new OllamaLocalLlmManager(httpClient, settings, ingredientRepo.Object);

        // Act
        var result = await sut.AnalyzeIngredientRequestAsync("Domat", CancellationToken.None);

        // Assert
        using var parsed = JsonDocument.Parse(result);
        var reason = parsed.RootElement.GetProperty("Reason").GetString();
        reason.Should().NotBeNull();
        reason!.Should().Contain("SİSTEM HATASI");
    }

    [Fact]
    public async Task AnalyzeIngredientRequestAsync_WhenHttpFails_ShouldReturnFallbackErrorJson()
    {
        // Arrange
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var httpClient = new HttpClient(handler);
        var settings = Options.Create(new OllamaSettings
        {
            BaseUrl = "http://localhost:11434",
            ModelName = "model",
            SystemPrompt = "prompt"
        });

        var ingredientRepo = new Mock<IIngredientRepository>();
        ingredientRepo
            .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Ingredient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Paginate<Ingredient> { Items = new List<Ingredient>() });

        var sut = new OllamaLocalLlmManager(httpClient, settings, ingredientRepo.Object);

        // Act
        var result = await sut.AnalyzeIngredientRequestAsync("Domat", CancellationToken.None);

        // Assert
        using var parsed = JsonDocument.Parse(result);
        var reason = parsed.RootElement.GetProperty("Reason").GetString();
        reason.Should().NotBeNull();
        reason!.Should().Contain("SİSTEM HATASI");
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

        public StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _responseFactory = responseFactory;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_responseFactory(request));
        }
    }
}
