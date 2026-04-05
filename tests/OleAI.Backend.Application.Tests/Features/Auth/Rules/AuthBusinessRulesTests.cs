using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Core.Security.Hashing;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Auth.Rules;

public class AuthBusinessRulesTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly AuthBusinessRules _rules;

    public AuthBusinessRulesTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _rules = new AuthBusinessRules(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task EmailCannotBeDuplicatedWhenRegistered_ShouldThrow_WhenEmailExists()
    {
        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(), null, false, true, default))
            .ReturnsAsync(new User { Email = "test@test.com" });

        var act = () => _rules.EmailCannotBeDuplicatedWhenRegistered("test@test.com");
        await act.Should().ThrowAsync<BusinessException>();
    }

    [Fact]
    public void UserShouldBeExistsWhenSelected_ShouldThrow_WhenUserIsNull()
    {
        var act = () => _rules.UserShouldBeExistsWhenSelected(null);
        act.Should().Throw<BusinessException>();
    }

    [Fact]
    public async Task UserAlreadyVerified_ShouldThrow_WhenStatusIsTrue()
    {
        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(), null, false, true, default))
            .ReturnsAsync(new User { IsEmailVerified = true });

        var act = () => _rules.UserAlreadyVerified("test@test.com");
        await act.Should().ThrowAsync<BusinessException>();
    }

    [Fact]
    public void VerificationCodeShouldBeValid_ShouldThrow_WhenCodeIsEmpty()
    {
        var act = () => _rules.VerificationCodeShouldBeValid("", "123");
        act.Should().Throw<BusinessException>();
    }

    [Fact]
    public void VerificationCodeShouldBeValid_ShouldThrow_WhenCodeMismatch()
    {
        var act = () => _rules.VerificationCodeShouldBeValid("456", "123");
        act.Should().Throw<BusinessException>();
    }

    [Fact]
    public void UserShouldBeVerifiedEmail_ShouldThrow_WhenStatusIsFalse()
    {
        var act = () => _rules.UserShouldBeVerifiedEmail(new User { IsEmailVerified = false });
        act.Should().Throw<BusinessException>();
    }

    [Fact]
    public void UserShouldBeActive_ShouldThrow_WhenIsActiveIsFalse()
    {
        var act = () => _rules.UserShouldBeActive(new User { IsActive = false });
        act.Should().Throw<BusinessException>();
    }

    [Fact]
    public void UserShouldBeActive_ShouldNotThrow_WhenIsActiveIsTrue()
    {
        var act = () => _rules.UserShouldBeActive(new User { IsActive = true });
        act.Should().NotThrow();
    }

    [Fact]
    public void PasswordAndEmailShouldBeValid_ShouldThrow_WhenPasswordWrong()
    {
        // Actually this needs HashingHelper verification, which is static.
        // We can just pass wrong inputs.
        var act = () => _rules.PasswordAndEmailShouldBeValid("wrong", new byte[1], new byte[1]);
        act.Should().Throw<BusinessException>();
    }

    // ─── ValidateCredentials (User Enumeration & Timing Attack mitigation) ───

    [Fact]
    public void ValidateCredentials_ShouldReturnFalse_WhenUserIsNull()
    {
        // Even when user doesn't exist, PBKDF2 runs on dummy values to prevent timing attacks.
        var result = _rules.ValidateCredentials(null, "anyPassword");

        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateCredentials_ShouldReturnFalse_WhenPasswordIsWrong()
    {
        HashingHelper.CreatePasswordHash("correctPassword", out byte[] hash, out byte[] salt);
        var user = new User
        {
            Email = "test@test.com",
            PasswordHash = hash,
            PasswordSalt = salt
        };

        var result = _rules.ValidateCredentials(user, "wrongPassword");

        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateCredentials_ShouldReturnTrue_WhenCredentialsAreCorrect()
    {
        HashingHelper.CreatePasswordHash("correctPassword", out byte[] hash, out byte[] salt);
        var user = new User
        {
            Email = "test@test.com",
            PasswordHash = hash,
            PasswordSalt = salt
        };

        var result = _rules.ValidateCredentials(user, "correctPassword");

        result.Should().BeTrue();
    }

    // ─── RefreshToken ───

    [Fact]
    public void RefreshTokenShouldBeActive_ShouldThrow_WhenNull()
    {
        var act = () => _rules.RefreshTokenShouldBeActive(null);
        act.Should().Throw<AuthenticationException>();
    }

    [Fact]
    public void RefreshTokenShouldBeActive_ShouldThrow_WhenNotActive()
    {
        // IsActive is computed: !IsExpired && !IsRevoked
        var token = new RefreshToken { Expires = DateTime.UtcNow.AddDays(-1) }; // Expired
        var act = () => _rules.RefreshTokenShouldBeActive(token);
        act.Should().Throw<AuthenticationException>();
    }

    // ─── ValidateAndTrackOtpAsync (Brute-Force mitigation) ───

    [Fact]
    public async Task ValidateAndTrackOtpAsync_ShouldThrow_WhenCodeIsEmpty()
    {
        var cacheMock = new Mock<ICacheService>();
        var act = async () => await _rules.ValidateAndTrackOtpAsync(cacheMock.Object, "OTP:Key", "", "123");
        await act.Should().ThrowAsync<BusinessException>().WithMessage(AuthMessages.VerificationCodeIsIncorrectOrExpired);
    }

    [Fact]
    public async Task ValidateAndTrackOtpAsync_ShouldIncrementAndThrow_WhenCodeMismatchAndUnderLimit()
    {
        var cacheMock = new Mock<ICacheService>();
        cacheMock.Setup(c => c.GetAsync<string>("OTP:Key:Attempts")).ReturnsAsync("1"); // 1 previous fail

        var act = async () => await _rules.ValidateAndTrackOtpAsync(cacheMock.Object, "OTP:Key", "456", "123");

        await act.Should().ThrowAsync<BusinessException>().WithMessage(AuthMessages.VerificationCodeIsIncorrectOrExpired);
        cacheMock.Verify(c => c.SetAsync("OTP:Key:Attempts", "2", It.IsAny<TimeSpan?>(), null), Times.Once);
    }

    [Fact]
    public async Task ValidateAndTrackOtpAsync_ShouldRemoveAndThrowLockout_WhenLimitReached()
    {
        var cacheMock = new Mock<ICacheService>();
        cacheMock.Setup(c => c.GetAsync<string>("OTP:Key:Attempts")).ReturnsAsync("2"); // limit is 3, this is the 3rd fail

        var act = async () => await _rules.ValidateAndTrackOtpAsync(cacheMock.Object, "OTP:Key", "456", "123");

        await act.Should().ThrowAsync<BusinessException>().WithMessage(AuthMessages.TooManyOtpAttempts);
        cacheMock.Verify(c => c.RemoveAsync("OTP:Key"), Times.Once);
        cacheMock.Verify(c => c.RemoveAsync("OTP:Key:Attempts"), Times.Once);
    }

    [Fact]
    public async Task ValidateAndTrackOtpAsync_ShouldClearAttemptsAndNotThrow_WhenCodeMatches()
    {
        var cacheMock = new Mock<ICacheService>();
        var act = async () => await _rules.ValidateAndTrackOtpAsync(cacheMock.Object, "OTP:Key", "123", "123");

        await act.Should().NotThrowAsync();
        cacheMock.Verify(c => c.RemoveAsync("OTP:Key:Attempts"), Times.Once);
    }
}
