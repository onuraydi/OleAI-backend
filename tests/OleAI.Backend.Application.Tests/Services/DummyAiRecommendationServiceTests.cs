using FluentAssertions;
using OleAI.Backend.Application.Services.AiRecommendation;
using Xunit;

namespace OleAI.Backend.Application.Tests.Services;

public class DummyAiRecommendationServiceTests
{
    [Fact]
    public async Task GetRecommendationsAsync_ShouldReturnMockResponse()
    {
        // Arrange
        var service = new DummyAiRecommendationService();
        var payload = new AiRecommendationPayload();

        // Act
        var result = await service.GetRecommendationsAsync(payload);

        // Assert
        result.Should().NotBeNull();
        result.Recipes.Should().BeEmpty();
    }
}
