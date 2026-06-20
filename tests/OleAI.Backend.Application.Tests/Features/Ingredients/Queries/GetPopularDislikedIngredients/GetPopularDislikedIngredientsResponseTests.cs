using FluentAssertions;
using OleAI.Backend.Application.Features.Ingredients.Queries.GetPopularDislikedIngredients;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Ingredients.Queries.GetPopularDislikedIngredients;

public class GetPopularDislikedIngredientsResponseTests
{
    [Fact]
    public void Properties_ShouldBeGettableAndSettable()
    {
        var dto = new GetPopularDislikedIngredientsResponse();
        var id = System.Guid.NewGuid();
        dto.Id = id;
        dto.Name = "Test";
        dto.CategoryName = "Cat";

        dto.Id.Should().Be(id);
        dto.Name.Should().Be("Test");
        dto.CategoryName.Should().Be("Cat");
    }
}
