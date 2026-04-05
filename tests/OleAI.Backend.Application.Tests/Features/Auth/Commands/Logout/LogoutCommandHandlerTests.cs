using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Auth.Commands.Logout;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Auth.Commands.Logout;

public class LogoutCommandHandlerTests
{
    private readonly Mock<IRefreshTokenRepository> _mockRefreshTokenRepository;
    private readonly LogoutCommandHandler _handler;

    public LogoutCommandHandlerTests()
    {
        _mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        _handler = new LogoutCommandHandler(_mockRefreshTokenRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenRefreshTokenIsEmpty_ShouldReturnSuccessWithoutRepositoryCall()
    {
        var command = new LogoutCommand { RefreshToken = string.Empty, IpAddress = "127.0.0.1" };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Message.Should().Be(AuthMessages.LogoutSuccesfullyMessage);
        _mockRefreshTokenRepository.Verify(r => r.GetAsync(It.IsAny<Expression<Func<OleAI.Backend.Domain.Entities.RefreshToken, bool>>>(), null, false, true, It.IsAny<CancellationToken>()), Times.Never);
        _mockRefreshTokenRepository.Verify(r => r.UpdateAsync(It.IsAny<OleAI.Backend.Domain.Entities.RefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenActiveTokenExists_ShouldRevokeAndUpdateToken()
    {
        var refreshToken = new OleAI.Backend.Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Token = "refresh-token",
            Expires = DateTime.UtcNow.AddDays(2),
            CreatedByIp = "127.0.0.1"
        };

        var command = new LogoutCommand { RefreshToken = "refresh-token", IpAddress = "10.1.1.1" };

        _mockRefreshTokenRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<OleAI.Backend.Domain.Entities.RefreshToken, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);

        _mockRefreshTokenRepository
            .Setup(r => r.UpdateAsync(refreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Message.Should().Be(AuthMessages.LogoutSuccesfullyMessage);
        refreshToken.Revoked.Should().NotBeNull();
        refreshToken.RevokedByIp.Should().Be("10.1.1.1");
        refreshToken.ReasonRevoked.Should().Be(AuthMessages.LogoutSuccesfullyWithUserRequestMessage);
        _mockRefreshTokenRepository.Verify(r => r.UpdateAsync(refreshToken, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenTokenNotFound_ShouldReturnSuccessWithoutUpdate()
    {
        var command = new LogoutCommand { RefreshToken = "not-found", IpAddress = "127.0.0.1" };

        _mockRefreshTokenRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<OleAI.Backend.Domain.Entities.RefreshToken, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OleAI.Backend.Domain.Entities.RefreshToken?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Message.Should().Be(AuthMessages.LogoutSuccesfullyMessage);
        _mockRefreshTokenRepository.Verify(r => r.UpdateAsync(It.IsAny<OleAI.Backend.Domain.Entities.RefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
