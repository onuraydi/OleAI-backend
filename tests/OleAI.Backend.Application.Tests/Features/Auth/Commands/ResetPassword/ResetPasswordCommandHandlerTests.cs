using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Auth.Commands.ResetPassword;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Features.Auth.Dtos.Requests;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly AuthBusinessRules _authBusinessRules;
    private readonly ResetPasswordCommandHandler _handler;

    public ResetPasswordCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockCacheService = new Mock<ICacheService>();
        _authBusinessRules = new AuthBusinessRules(_mockUserRepository.Object);

        _handler = new ResetPasswordCommandHandler(
            _mockUserRepository.Object,
            _mockCacheService.Object,
            _authBusinessRules
        );
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldResetPassword()
    {
        // Arrange
        var request = new ResetPasswordRequest { Email = "test@example.com", OtpCode = "123456", NewPassword = "newPassword123" };
        var command = new ResetPasswordCommand { Request = request };

        var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", PasswordHash = Array.Empty<byte>(), PasswordSalt = Array.Empty<byte>() };

        _mockUserRepository.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
            It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>(),
            false,
            true,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var redisKey = $"OTP:Reset{request.Email}";
        _mockCacheService.Setup(c => c.GetAsync<string>(redisKey))
            .ReturnsAsync("123456");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Message.Should().Be(AuthMessages.PasswordResetSuccessfully);

        user.PasswordHash.Should().NotBeEmpty();
        user.PasswordSalt.Should().NotBeEmpty();

        _mockUserRepository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        _mockCacheService.Verify(c => c.RemoveAsync(redisKey), Times.Once);
    }
}
