using FluentAssertions;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Dtos.Responses;

public class MyCommentResponseTests
{
    [Fact]
    public void Properties_ShouldBeGettableAndSettable()
    {
        var dto = new MyCommentResponse();
        var id = System.Guid.NewGuid();
        var recipeId = System.Guid.NewGuid();
        var date = System.DateTime.UtcNow;

        dto.Id = id;
        dto.RecipeId = recipeId;
        dto.RecipeName = "Recipe";
        dto.RecipeImageUrl = "url";
        dto.Content = "Content";
        dto.Rating = 5;
        dto.CreatedDate = date;
        dto.ParentCommentId = id;
        dto.ParentCommentContent = "Parent";

        dto.Id.Should().Be(id);
        dto.RecipeId.Should().Be(recipeId);
        dto.RecipeName.Should().Be("Recipe");
        dto.RecipeImageUrl.Should().Be("url");
        dto.Content.Should().Be("Content");
        dto.Rating.Should().Be(5);
        dto.CreatedDate.Should().Be(date);
        dto.ParentCommentId.Should().Be(id);
        dto.ParentCommentContent.Should().Be("Parent");
    }
}
