using FluentAssertions;
using OleAI.Backend.Application.Features.Recipes.Queries.GetMyLikedRecipes;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Queries.GetMyLikedRecipes;

public class GetMyLikedRecipesQueryTests
{
    [Fact]
    public void Properties_ShouldBeGettableAndSettable()
    {
        var query = new GetMyLikedRecipesQuery();

        query.Page = 2;
        query.Size = 20;

        query.Page.Should().Be(2);
        query.Size.Should().Be(20);
    }
}
