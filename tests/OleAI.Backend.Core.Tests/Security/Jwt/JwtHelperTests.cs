using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using OleAI.Backend.Core.Security.Jwt;
using Xunit;

namespace OleAI.Backend.Core.Tests.Security.Jwt;

public class JwtHelperTests
{
    private readonly IConfiguration _configuration;

    public JwtHelperTests()
    {
        var myConfiguration = new Dictionary<string, string?>
        {
            {"TokenOptions:Audience", "OleAI_Audience"},
            {"TokenOptions:Issuer", "OleAI_Issuer"},
            {"TokenOptions:AccessTokenExpiration", "60"},
            {"TokenOptions:SecurityKey", "MySuperSecretKeyForTestingJwtTokenGeneration12345!_ThisKeyMustBeAtLeast64BytesLong"},
            {"TokenOptions:RefreshTokenExpiration", "14"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(myConfiguration)
            .Build();
    }

    [Fact]
    public void Constructor_WhenTokenOptionsNotFound_ShouldThrowArgumentNullException()
    {
        // Arrange
        var emptyConfig = new ConfigurationBuilder().AddInMemoryCollection().Build();

        // Act
        Action act = () => new JwtHelper(emptyConfig);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*TokenOptions configuration not found.*");
    }

    [Fact]
    public void CreateToken_ShouldReturnValidJwtToken()
    {
        // Arrange
        var jwtHelper = new JwtHelper(_configuration);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Email, "test@oleai.com")
        };

        // Act
        var result = jwtHelper.CreateToken(claims);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrWhiteSpace();
        result.Expriration.Should().BeAfter(DateTime.UtcNow);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(result.Token);

        jwtToken.Issuer.Should().Be("OleAI_Issuer");
        jwtToken.Audiences.First().Should().Be("OleAI_Audience");
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == "test@oleai.com");
    }

    [Fact]
    public void CreateRefreshToken_ShouldReturnValidTokenAndExpiration()
    {
        // Arrange
        var jwtHelper = new JwtHelper(_configuration);

        // Act
        var result = jwtHelper.CreateRefreshToken();

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrWhiteSpace();
        result.Expiration.Should().BeAfter(DateTime.UtcNow.AddDays(13));
    }
}
