using FluentAssertions;
using OleAI.Backend.Application.Features.Users.Dtos.Responses;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Users.Dtos.Responses;

public class UserProfileResponseTests
{
    [Fact]
    public void Properties_ShouldBeGettableAndSettable()
    {
        var response = new UserProfileResponse();
        var id = System.Guid.NewGuid();
        var date = System.DateTime.UtcNow;

        response.Id = id;
        response.FirstName = "First";
        response.LastName = "Last";
        response.Email = "Email";
        response.Username = "User";
        response.ProfileImageUrl = "Url";
        response.IsOnboarded = true;
        response.BirthDate = date;

        response.Id.Should().Be(id);
        response.FirstName.Should().Be("First");
        response.LastName.Should().Be("Last");
        response.Email.Should().Be("Email");
        response.Username.Should().Be("User");
        response.ProfileImageUrl.Should().Be("Url");
        response.IsOnboarded.Should().BeTrue();
        response.BirthDate.Should().Be(date);
    }
}
