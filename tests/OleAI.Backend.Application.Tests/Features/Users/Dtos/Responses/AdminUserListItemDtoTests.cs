using FluentAssertions;
using OleAI.Backend.Application.Features.Users.Dtos.Responses;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Users.Dtos.Responses;

public class AdminUserListItemDtoTests
{
    [Fact]
    public void Properties_ShouldBeGettableAndSettable()
    {
        var dto = new AdminUserListItemDto();

        var id = Guid.NewGuid();
        var date = DateTime.UtcNow;

        dto.Id = id;
        dto.FirstName = "John";
        dto.LastName = "Doe";
        dto.Email = "john@example.com";
        dto.IsActive = true;
        dto.LockoutEnd = date;
        dto.Roles.Add("Admin");

        dto.Id.Should().Be(id);
        dto.FirstName.Should().Be("John");
        dto.LastName.Should().Be("Doe");
        dto.Email.Should().Be("john@example.com");
        dto.IsActive.Should().BeTrue();
        dto.LockoutEnd.Should().Be(date);
        dto.Roles.Should().ContainSingle().Which.Should().Be("Admin");
    }
}
