using FluentAssertions;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Domain.Tests.Entities;

public class RefreshTokenTests
{
    // ──────────────────────────────────────────────
    // IsExpired
    // ──────────────────────────────────────────────

    [Fact]
    public void IsExpired_WhenExpiresInPast_ShouldBeTrue()
    {
        var token = new RefreshToken { Expires = DateTime.UtcNow.AddSeconds(-1) };

        token.IsExpired.Should().BeTrue();
    }

    [Fact]
    public void IsExpired_WhenExpiresInFuture_ShouldBeFalse()
    {
        var token = new RefreshToken { Expires = DateTime.UtcNow.AddDays(7) };

        token.IsExpired.Should().BeFalse();
    }

    // ──────────────────────────────────────────────
    // IsRevoked
    // ──────────────────────────────────────────────

    [Fact]
    public void IsRevoked_WhenRevokedHasValue_ShouldBeTrue()
    {
        var token = new RefreshToken { Revoked = DateTime.UtcNow };

        token.IsRevoked.Should().BeTrue();
    }

    [Fact]
    public void IsRevoked_WhenRevokedIsNull_ShouldBeFalse()
    {
        var token = new RefreshToken { Revoked = null };

        token.IsRevoked.Should().BeFalse();
    }

    // ──────────────────────────────────────────────
    // IsActive  (not revoked AND not expired)
    // ──────────────────────────────────────────────

    [Fact]
    public void IsActive_WhenNotRevokedAndNotExpired_ShouldBeTrue()
    {
        var token = new RefreshToken
        {
            Expires = DateTime.UtcNow.AddDays(7),
            Revoked = null
        };

        token.IsActive.Should().BeTrue();
    }

    [Fact]
    public void IsActive_WhenRevoked_ShouldBeFalse()
    {
        var token = new RefreshToken
        {
            Expires = DateTime.UtcNow.AddDays(7),
            Revoked = DateTime.UtcNow.AddMinutes(-1) // revoked
        };

        token.IsActive.Should().BeFalse();
    }

    [Fact]
    public void IsActive_WhenExpired_ShouldBeFalse()
    {
        var token = new RefreshToken
        {
            Expires = DateTime.UtcNow.AddDays(-1), // expired
            Revoked = null
        };

        token.IsActive.Should().BeFalse();
    }

    [Fact]
    public void IsActive_WhenBothRevokedAndExpired_ShouldBeFalse()
    {
        var token = new RefreshToken
        {
            Expires = DateTime.UtcNow.AddDays(-1),
            Revoked = DateTime.UtcNow.AddDays(-2)
        };

        token.IsActive.Should().BeFalse();
    }
}
