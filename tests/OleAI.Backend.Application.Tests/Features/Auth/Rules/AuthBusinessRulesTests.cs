using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Services.Repositories;
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
    public void PasswordAndEmailShouldBeValid_ShouldThrow_WhenPasswordWrong()
    {
        // Actually this needs HashingHelper verification, which is static.
        // We can just pass wrong inputs.
        var act = () => _rules.PasswordAndEmailShouldBeValid("wrong", new byte[1], new byte[1]);
        act.Should().Throw<BusinessException>();
    }

    // ─── ValidateCredentials (User Enumeration & Timing Attack mitigation) ───

    [Fact]
    public void ValidateCredentials_ShouldThrow_WhenUserIsNull()
    {
        // Even when user doesn't exist, PBKDF2 runs on dummy values before throwing.
        var act = () => _rules.ValidateCredentials(null, "anyPassword");

        act.Should().Throw<BusinessException>()
            .WithMessage(AuthMessages.InvalidCredentials);
    }

    [Fact]
    public void ValidateCredentials_ShouldThrow_WhenPasswordIsWrong()
    {
        HashingHelper.CreatePasswordHash("correctPassword", out byte[] hash, out byte[] salt);
        var user = new User
        {
            Email = "test@test.com",
            PasswordHash = hash,
            PasswordSalt = salt
        };

        var act = () => _rules.ValidateCredentials(user, "wrongPassword");

        act.Should().Throw<BusinessException>()
            .WithMessage(AuthMessages.InvalidCredentials);
    }

    [Fact]
    public void ValidateCredentials_ShouldNotThrow_WhenCredentialsAreCorrect()
    {
        HashingHelper.CreatePasswordHash("correctPassword", out byte[] hash, out byte[] salt);
        var user = new User
        {
            Email = "test@test.com",
            PasswordHash = hash,
            PasswordSalt = salt
        };

        var act = () => _rules.ValidateCredentials(user, "correctPassword");

        act.Should().NotThrow();
    }

    [Fact]
    public void ValidateCredentials_ShouldReturn_SameErrorMessage_ForBothScenarios()
    {
        // Scenario 1: User does not exist
        string? messageWhenUserNotFound = null;
        try { _rules.ValidateCredentials(null, "anyPassword"); }
        catch (BusinessException ex) { messageWhenUserNotFound = ex.Message; }

        // Scenario 2: User exists but wrong password
        HashingHelper.CreatePasswordHash("correct", out byte[] hash, out byte[] salt);
        var user = new User { PasswordHash = hash, PasswordSalt = salt };

        string? messageWhenWrongPassword = null;
        try { _rules.ValidateCredentials(user, "wrong"); }
        catch (BusinessException ex) { messageWhenWrongPassword = ex.Message; }

        // Both messages must be identical — no user enumeration leakage
        messageWhenUserNotFound.Should().Be(messageWhenWrongPassword);
        messageWhenUserNotFound.Should().Be(AuthMessages.InvalidCredentials);
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
}
