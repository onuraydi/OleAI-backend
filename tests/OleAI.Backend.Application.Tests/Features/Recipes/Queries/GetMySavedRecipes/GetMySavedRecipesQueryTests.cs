using FluentAssertions;
using OleAI.Backend.Application.Features.Recipes.Queries.GetMySavedRecipes;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Queries.GetMySavedRecipes;

public class GetMySavedRecipesQueryTests
{
    [Fact]
    public void Properties_ShouldBeGettableAndSettable()
    {
        var query = new GetMySavedRecipesQuery();

        query.Page = 2;
        query.Size = 20;

        query.Page.Should().Be(2);
        query.Size.Should().Be(20);
    }
}
