using FluentAssertions;
using OleAI.Backend.Application.Features.Users.Queries.GetUserStatistics;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Users.Queries.GetUserStatistics;

public class GetUserStatisticsResponseTests
{
    [Fact]
    public void Properties_ShouldBeGettableAndSettable()
    {
        var dto = new GetUserStatisticsResponse();
        dto.TotalRecipesCreated = 1;
        dto.TotalRecipesSaved = 2;
        dto.TotalPantryItems = 3;
        dto.TotalRecipesLiked = 4;

        dto.TotalRecipesCreated.Should().Be(1);
        dto.TotalRecipesSaved.Should().Be(2);
        dto.TotalPantryItems.Should().Be(3);
        dto.TotalRecipesLiked.Should().Be(4);
    }
}
