using FluentAssertions;
using OleAI.Backend.Application.Features.Pantry.Dtos.Responses;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Pantry.Dtos.Responses;

public class CategorizedPopularIngredientsResponseTests
{
    [Fact]
    public void Properties_ShouldBeGettableAndSettable()
    {
        var dto = new CategorizedPopularIngredientsResponse();
        var id = System.Guid.NewGuid();
        dto.CategoryId = id;
        dto.CategoryName = "Spices";
        dto.Ingredients = new System.Collections.Generic.List<IngredientSearchResultResponse>();

        dto.CategoryId.Should().Be(id);
        dto.CategoryName.Should().Be("Spices");
        dto.Ingredients.Should().NotBeNull();
    }
}
