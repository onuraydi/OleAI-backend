using FluentAssertions;
using OleAI.Backend.Application.Features.Recipes.Queries.GetRecipesForAi;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Queries.GetRecipesForAi;

public class GetRecipesForAiQueryTests
{
    [Fact]
    public void DtoProperties_ShouldBeGettableAndSettable()
    {
        var dto = new RecipeForAiDto();
        var id = System.Guid.NewGuid();
        var date = System.DateTime.UtcNow;

        dto.Id = id;
        dto.Title = "Title";
        dto.Category = "Cat";
        dto.CreatedAt = date;
        dto.IsPublished = true;
        dto.Popularity = 10;
        dto.Ingredients = new System.Collections.Generic.List<OleAI.Backend.Application.Services.AiRecommendation.RecipeIngredientAiDto>();

        dto.Id.Should().Be(id);
        dto.Title.Should().Be("Title");
        dto.Category.Should().Be("Cat");
        dto.CreatedAt.Should().Be(date);
        dto.IsPublished.Should().BeTrue();
        dto.Popularity.Should().Be(10);
        dto.Ingredients.Should().NotBeNull();
    }
}
