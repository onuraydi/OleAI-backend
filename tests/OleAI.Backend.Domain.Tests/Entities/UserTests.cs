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

        // Assert
        user.UserOperationClaims.Should().NotBeNull();
        user.RefreshTokens.Should().NotBeNull();
        user.UserDietaryPreferences.Should().NotBeNull();
        user.UserAllergies.Should().NotBeNull();
        user.UserDislikedIngredients.Should().NotBeNull();
        user.UserIngredients.Should().NotBeNull();
        user.Interactions.Should().NotBeNull();
        user.Notifications.Should().NotBeNull();
        user.UserDevices.Should().NotBeNull();
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
        user.Username.Should().BeEmpty();
        user.IsOnboarded.Should().BeFalse();
        user.IsEmailVerified.Should().BeFalse();
        user.IsActive.Should().BeTrue();
        user.TermsAccepted.Should().BeFalse();
        user.FailedLoginAttempts.Should().Be(0);
        user.PasswordHash.Should().BeEmpty();
        user.PasswordSalt.Should().BeEmpty();
        user.BirthDate.Should().BeNull();
        user.ProfileImageUrl.Should().BeNull();
        user.LockoutEnd.Should().BeNull();
    }
}
