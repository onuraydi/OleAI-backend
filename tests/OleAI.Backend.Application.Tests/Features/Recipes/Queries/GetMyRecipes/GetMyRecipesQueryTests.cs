using FluentAssertions;
using OleAI.Backend.Application.Features.Recipes.Queries.GetMyRecipes;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Queries.GetMyRecipes;

public class GetMyRecipesQueryTests
{
    [Fact]
    public void Properties_ShouldBeGettableAndSettable()
    {
        var query = new GetMyRecipesQuery();

        query.Page = 2;
        query.Size = 20;

        query.Page.Should().Be(2);
        query.Size.Should().Be(20);
    }
}
