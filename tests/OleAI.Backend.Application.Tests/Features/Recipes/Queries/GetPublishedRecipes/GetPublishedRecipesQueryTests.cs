using FluentAssertions;
using OleAI.Backend.Application.Features.Recipes.Queries.GetPublishedRecipes;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Queries.GetPublishedRecipes;

public class GetPublishedRecipesQueryTests
{
    [Fact]
    public void Properties_ShouldBeGettableAndSettable()
    {
        var query = new GetPublishedRecipesQuery();

        query.Page = 2;
        query.Size = 20;

        query.Page.Should().Be(2);
        query.Size.Should().Be(20);
    }
}
