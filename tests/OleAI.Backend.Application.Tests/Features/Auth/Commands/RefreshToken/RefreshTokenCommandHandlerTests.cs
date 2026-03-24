using System.Security.Claims;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Auth.Commands.RefreshToken;
using OleAI.Backend.Application.Features.Auth.Dtos.Requests;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Helpers;
using OleAI.Backend.Core.Security.Jwt;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<IRefreshTokenRepository> _mockRefreshTokenRepo;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ITokenHelper> _mockTokenHelper;
    private readonly AuthBusinessRules _authBusinessRules;
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _mockRefreshTokenRepo = new Mock<IRefreshTokenRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockTokenHelper = new Mock<ITokenHelper>();
        _authBusinessRules = new AuthBusinessRules(_mockUserRepository.Object);

        _handler = new RefreshTokenCommandHandler(
            _mockRefreshTokenRepo.Object,
            _mockUserRepository.Object,
            _mockTokenHelper.Object,
            _authBusinessRules
        );
    }

    [Fact]
    public async Task Handle_ValidToken_ShouldReturnNewTokens()
    {
        // Arrange
        string oldTokenStr = "old-token";
        var command = new RefreshTokenCommand
        {
            Request = new RefreshTokenRequest { Token = oldTokenStr, IpAddress = "127.0.0.1" }
        };

        var refreshTokenDb = new OleAI.Backend.Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Token = oldTokenStr,
            Expires = DateTime.UtcNow.AddDays(7),
            CreatedByIp = "127.0.0.1",
            Revoked = null
        };

        var user = new User
        {
            Id = refreshTokenDb.UserId,
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            UserOperationClaims = new List<UserOperationClaim>
            {
                new UserOperationClaim { operationClaim = new OperationClaim { Name = "User" } }
            }
        };

        var newAccessTokenStr = new AccessToken { Token = "new-access", Expriration = DateTime.UtcNow.AddMinutes(15) };
        var newRefreshTokenCore = new OleAI.Backend.Core.Security.Jwt.RefreshToken { Token = "new-refresh", Expiration = DateTime.UtcNow.AddDays(7) };

        _mockRefreshTokenRepo.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<OleAI.Backend.Domain.Entities.RefreshToken, bool>>>(),
            It.IsAny<Func<IQueryable<OleAI.Backend.Domain.Entities.RefreshToken>, IIncludableQueryable<OleAI.Backend.Domain.Entities.RefreshToken, object>>>(),
            false,
            true,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshTokenDb);

        _mockUserRepository.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
            It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>(),
            false,
            true,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mockTokenHelper.Setup(t => t.CreateToken(It.IsAny<IEnumerable<Claim>>()))
            .Returns(newAccessTokenStr);

        _mockTokenHelper.Setup(t => t.CreateRefreshToken())
            .Returns(newRefreshTokenCore);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be(newAccessTokenStr);
        result.RefreshToken.Should().Be(newRefreshTokenCore);

        refreshTokenDb.Revoked.Should().NotBeNull();
        refreshTokenDb.ReplacedByToken.Should().Be("new-refresh");

        _mockRefreshTokenRepo.Verify(r => r.UpdateAsync(refreshTokenDb, It.IsAny<CancellationToken>()), Times.Once);
        _mockRefreshTokenRepo.Verify(r => r.AddAsync(It.Is<OleAI.Backend.Domain.Entities.RefreshToken>(x => x.Token == "new-refresh"), It.IsAny<CancellationToken>()), Times.Once);
    }
}
