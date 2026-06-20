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
        query.SearchTerm = "T";
        query.CategoryId = 1;
        query.Difficulty = OleAI.Backend.Domain.Enums.RecipeDifficulty.Easy;
        query.MaxTotalTime = 30;
        query.TagId = 1;
        query.MinRating = 4.0;
        query.Visibility = OleAI.Backend.Domain.Enums.RecipeVisibility.Public;
        query.SortBy = "newest";

        query.Page.Should().Be(2);
        query.Size.Should().Be(20);
        query.SearchTerm.Should().Be("T");
        query.CategoryId.Should().Be(1);
        query.Difficulty.Should().Be(OleAI.Backend.Domain.Enums.RecipeDifficulty.Easy);
        query.MaxTotalTime.Should().Be(30);
        query.TagId.Should().Be(1);
        query.MinRating.Should().Be(4.0);
        query.Visibility.Should().Be(OleAI.Backend.Domain.Enums.RecipeVisibility.Public);
        query.SortBy.Should().Be("newest");
    }
}
