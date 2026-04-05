using FluentAssertions;
using OleAI.Backend.Core.Security.Jwt;

namespace OleAI.Backend.Core.Tests.Security.Jwt;

public class TokenModelsTests
{
    [Fact]
    public void AccessToken_ShouldStoreTokenAndExpiration()
    {
        var expiration = DateTime.UtcNow.AddMinutes(15);
        var token = new AccessToken
        {
            Token = "access-token",
            Expriration = expiration
        };

        token.Token.Should().Be("access-token");
        token.Expriration.Should().Be(expiration);
    }

    [Fact]
    public void RefreshToken_ShouldStoreTokenAndExpiration()
    {
        var expiration = DateTime.UtcNow.AddDays(7);
        var token = new RefreshToken
        {
            Token = "refresh-token",
            Expiration = expiration
        };

        token.Token.Should().Be("refresh-token");
        token.Expiration.Should().Be(expiration);
    }

    [Fact]
    public void TokenOptions_ShouldHaveExpectedDefaults_AndAllowOverrides()
    {
        var options = new TokenOptions
        {
            Audience = "aud",
            Issuer = "issuer",
            SecurityKey = "secret",
            AccessTokenExpiration = 15,
            RefreshTokenExpiration = 7
        };

        options.Audience.Should().Be("aud");
        options.Issuer.Should().Be("issuer");
        options.SecurityKey.Should().Be("secret");
        options.AccessTokenExpiration.Should().Be(15);
        options.RefreshTokenExpiration.Should().Be(7);
        options.MaxActiveTokens.Should().Be(5);
    }
}
