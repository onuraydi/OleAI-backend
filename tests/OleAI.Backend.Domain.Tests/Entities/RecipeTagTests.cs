using FluentAssertions;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Domain.Tests.Entities;

public class RecipeTagTests
{
    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        var id = Guid.NewGuid();
        var recipeId = Guid.NewGuid();
        var tagId = 1;

        var tag = new RecipeTag(id, recipeId, tagId);

        tag.Id.Should().Be(id);
        tag.RecipeId.Should().Be(recipeId);
        tag.TagId.Should().Be(tagId);
    }
}
