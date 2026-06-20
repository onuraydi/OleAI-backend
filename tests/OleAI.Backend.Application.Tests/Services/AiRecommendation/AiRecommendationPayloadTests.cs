using FluentAssertions;
using OleAI.Backend.Application.Services.AiRecommendation;
using Xunit;

namespace OleAI.Backend.Application.Tests.Services.AiRecommendation;

public class AiRecommendationPayloadTests
{
    [Fact]
    public void Properties_ShouldBeGettableAndSettable()
    {
        var payload = new AiRecommendationPayload();

        payload.UserId = Guid.NewGuid();
        payload.PantryItems = new List<PantryItemDto>();
        payload.Interactions = new List<RecipeInteractionDto>();
        payload.DietaryPreferences = new List<string> { "Vegan" };
        payload.ExcludedIngredients = new List<string> { "Nut" };

        payload.UserId.Should().NotBeEmpty();
        payload.PantryItems.Should().NotBeNull();
        payload.Interactions.Should().NotBeNull();
        payload.DietaryPreferences.Should().Contain("Vegan");
        payload.ExcludedIngredients.Should().Contain("Nut");
    }
}
