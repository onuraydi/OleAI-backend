using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Auth.Commands.ResetPassword;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Features.Auth.Dtos.Requests;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
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
        result.Message.Should().Be(AuthMessages.AccountRecoverySuccess);

        user.PasswordHash.Should().NotBeEmpty();
        user.PasswordSalt.Should().NotBeEmpty();

        _mockUserRepository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        _mockCacheService.Verify(c => c.RemoveAsync(redisKey), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ShouldThrowSameError_AsInvalidOtp()
    {
        // Arrange — user does not exist, so no OTP was ever generated → cache returns null
        var command = new ResetPasswordCommand
        {
            Request = new ResetPasswordRequest { Email = "nobody@example.com", OtpCode = "123456", NewPassword = "newPass" }
        };

        _mockUserRepository.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
            It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>(),
            false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Cache returns null — no OTP exists for this non-existent user
        _mockCacheService.Setup(c => c.GetAsync<string>(It.IsAny<string>()))
            .ReturnsAsync((string?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert — same error as invalid OTP, NOT a "user not found" message
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage(AuthMessages.VerificationCodeIsIncorrectOrExpired);
    }

    [Fact]
    public async Task Handle_UserNotFound_And_InvalidOtp_ShouldReturn_IdenticalErrorMessages()
    {
        // Scenario 1: Non-existent user (cache returns null)
        var commandNoUser = new ResetPasswordCommand
        {
            Request = new ResetPasswordRequest { Email = "ghost@example.com", OtpCode = "123456", NewPassword = "newPass" }
        };

        _mockUserRepository.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
            It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>(),
            false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _mockCacheService.Setup(c => c.GetAsync<string>(It.IsAny<string>()))
            .ReturnsAsync((string?)null);

        string? messageNoUser = null;
        try { await _handler.Handle(commandNoUser, CancellationToken.None); }
        catch (BusinessException ex) { messageNoUser = ex.Message; }

        // Scenario 2: Existing user, wrong OTP code
        var existingUser = new User { Id = Guid.NewGuid(), Email = "test@example.com" };
        var commandWrongOtp = new ResetPasswordCommand
        {
            Request = new ResetPasswordRequest { Email = "test@example.com", OtpCode = "999999", NewPassword = "newPass" }
        };

        _mockUserRepository.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
            It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>(),
            false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        _mockCacheService.Setup(c => c.GetAsync<string>(It.IsAny<string>()))
            .ReturnsAsync("123456"); // correct code is 123456 but user sent 999999

        string? messageWrongOtp = null;
        try { await _handler.Handle(commandWrongOtp, CancellationToken.None); }
        catch (BusinessException ex) { messageWrongOtp = ex.Message; }

        // The core anti-enumeration assertion: both messages MUST be identical
        messageNoUser.Should().Be(messageWrongOtp);
        messageNoUser.Should().Be(AuthMessages.VerificationCodeIsIncorrectOrExpired);
    }

    [Fact]
    public async Task Handle_InvalidOtpCode_ShouldThrowBusinessException()
    {
        // Arrange
        var request = new ResetPasswordRequest { Email = "test@example.com", OtpCode = "999999", NewPassword = "newPass" };
        var command = new ResetPasswordCommand { Request = request };

        var user = new User { Id = Guid.NewGuid(), Email = "test@example.com" };

        _mockUserRepository.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
            It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>(),
            false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Cache returns a DIFFERENT code → OTP validation fails
        _mockCacheService.Setup(c => c.GetAsync<string>(It.IsAny<string>()))
            .ReturnsAsync("123456");

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage(AuthMessages.VerificationCodeIsIncorrectOrExpired);
    }

    [Fact]
    public async Task Handle_ExpiredOtpCode_ShouldThrowBusinessException()
    {
        // Arrange – cache returns null (expired)
        var request = new ResetPasswordRequest { Email = "test@example.com", OtpCode = "123456", NewPassword = "newPass" };
        var command = new ResetPasswordCommand { Request = request };

        var user = new User { Id = Guid.NewGuid(), Email = "test@example.com" };

        _mockUserRepository.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
            It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>(),
            false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mockCacheService.Setup(c => c.GetAsync<string>(It.IsAny<string>()))
            .ReturnsAsync((string?)null); // code expired / not found in cache

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage(AuthMessages.VerificationCodeIsIncorrectOrExpired);
    }
}
