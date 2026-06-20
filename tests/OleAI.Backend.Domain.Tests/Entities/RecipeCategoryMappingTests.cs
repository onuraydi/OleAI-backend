using FluentAssertions;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Domain.Tests.Entities;

public class RecipeCategoryMappingTests
{
    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        var id = Guid.NewGuid();
        var recipeId = Guid.NewGuid();
        var categoryId = 1;

        var mapping = new RecipeCategoryMapping(id, recipeId, categoryId);

        mapping.Id.Should().Be(id);
        mapping.RecipeId.Should().Be(recipeId);
        mapping.RecipeCategoryId.Should().Be(categoryId);
    }
}
