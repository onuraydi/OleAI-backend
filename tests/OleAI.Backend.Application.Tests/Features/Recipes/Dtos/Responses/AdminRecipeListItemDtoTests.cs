using FluentAssertions;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Dtos.Responses;

public class AdminRecipeListItemDtoTests
{
    [Fact]
    public void Properties_ShouldBeGettableAndSettable()
    {
        var dto = new AdminRecipeListItemDto();

        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var date = DateTime.UtcNow;

        dto.Id = id;
        dto.UserId = userId;
        dto.UserName = "TestUser";
        dto.Title = "Test Title";
        dto.Status = "Published";
        dto.Visibility = "Public";
        dto.CreatedDate = date;

        dto.Id.Should().Be(id);
        dto.UserId.Should().Be(userId);
        dto.UserName.Should().Be("TestUser");
        dto.Title.Should().Be("Test Title");
        dto.Status.Should().Be("Published");
        dto.Visibility.Should().Be("Public");
        dto.CreatedDate.Should().Be(date);
    }
}
