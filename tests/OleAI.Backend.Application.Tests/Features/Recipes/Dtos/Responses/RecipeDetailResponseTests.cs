using FluentAssertions;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Dtos.Responses;

public class RecipeDetailResponseTests
{
    [Fact]
    public void Properties_ShouldBeGettableAndSettable()
    {
        var response = new RecipeDetailResponse();

        response.Id = Guid.NewGuid();
        response.UserId = Guid.NewGuid();
        response.Title = "Title";
        response.Description = "Desc";
        response.Difficulty = OleAI.Backend.Domain.Enums.RecipeDifficulty.Easy;
        response.PrepTime = 10;
        response.CookTime = 20;
        response.Servings = 4;
        response.Visibility = OleAI.Backend.Domain.Enums.RecipeVisibility.Public;
        response.Status = OleAI.Backend.Domain.Enums.RecipeStatus.PendingLlmApproval;
        response.CreatedDate = DateTime.UtcNow;
        response.UpdatedDate = DateTime.UtcNow;
        response.LlmReviewResultJson = "{}";

        response.Id.Should().NotBeEmpty();
        response.UserId.Should().NotBeEmpty();
        response.Title.Should().Be("Title");
        response.Description.Should().Be("Desc");
        response.Difficulty.Should().Be(OleAI.Backend.Domain.Enums.RecipeDifficulty.Easy);
        response.PrepTime.Should().Be(10);
        response.CookTime.Should().Be(20);
        response.Servings.Should().Be(4);
        response.Visibility.Should().Be(OleAI.Backend.Domain.Enums.RecipeVisibility.Public);
        response.Status.Should().Be(OleAI.Backend.Domain.Enums.RecipeStatus.PendingLlmApproval);
        response.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        response.UpdatedDate.Should().HaveValue();
        response.LlmReviewResultJson.Should().Be("{}");
    }
}
