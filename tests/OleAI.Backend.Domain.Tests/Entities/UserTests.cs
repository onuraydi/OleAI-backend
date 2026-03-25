using FluentAssertions;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void Constructor_ShouldInitializeCollections_NotNull()
    {
        // Act
        var user = new User();

        // Assert – both navigation collections must be initialized so EF
        // proxies and LINQ queries don't throw NullReferenceException.
        user.UserOperationClaims.Should().NotBeNull();
        user.RefreshTokens.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_ShouldInitializeCollectionsAsEmpty()
    {
        // Act
        var user = new User();

        // Assert – a fresh User has no claims or tokens
        user.UserOperationClaims.Should().BeEmpty();
        user.RefreshTokens.Should().BeEmpty();
    }

    [Fact]
    public void Properties_ShouldHaveCorrectDefaults()
    {
        // Act
        var user = new User();

        // Assert
        user.FirstName.Should().BeEmpty();
        user.LastName.Should().BeEmpty();
        user.Email.Should().BeEmpty();
        user.Status.Should().BeFalse();
        user.PasswordHash.Should().BeEmpty();
        user.PasswordSalt.Should().BeEmpty();
    }
}
