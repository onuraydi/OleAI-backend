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
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly AuthBusinessRules _businessRules;

    public AuthBusinessRulesTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _businessRules = new AuthBusinessRules(_mockUserRepository.Object);
    }

    [Fact]
    public async Task EmailCannotBeDuplicatedWhenRegistered_ShouldThrowWhenUserExists()
    {
        // Arrange
        string email = "test@test.com";
        _mockUserRepository.Setup(x => x.GetAsync(
            u => u.Email == email,
            null,
            false,
            true,
            default))
            .ReturnsAsync(new User { Email = email });

        // Act
        Func<Task> act = async () => await _businessRules.EmailCannotBeDuplicatedWhenRegistered(email);

        // Assert
        await act.Should().ThrowAsync<BusinessException>().WithMessage(AuthMessages.UserMailAlreadyExists);
    }

    [Fact]
    public async Task EmailCannotBeDuplicatedWhenRegistered_ShouldNotThrowWhenUserDoesNotExist()
    {
        // Arrange
        string email = "test@test.com";

        // Moq returns null by default if not setup, but let's be explicit
        _mockUserRepository.Setup(x => x.GetAsync(
            u => u.Email == email,
            null,
            false,
            true,
            default))
            .ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _businessRules.EmailCannotBeDuplicatedWhenRegistered(email);

        // Assert
        await act.Should().NotThrowAsync<BusinessException>();
    }

    [Fact]
    public void UserShouldBeExistsWhenSelected_ShouldThrowWhenUserIsNull()
    {
        // Act
        Action act = () => _businessRules.UserShouldBeExistsWhenSelected(null);

        // Assert
        act.Should().Throw<BusinessException>().WithMessage(AuthMessages.UserNotFound);
    }

    [Fact]
    public void UserShouldBeExistsWhenSelected_ShouldNotThrowWhenUserExists()
    {
        // Act
        Action act = () => _businessRules.UserShouldBeExistsWhenSelected(new User());

        // Assert
        act.Should().NotThrow<BusinessException>();
    }

    [Fact]
    public async Task UserAlreadyVerified_ShouldThrowWhenUserStatusIsTrue()
    {
        // Arrange
        string email = "test@test.com";
        _mockUserRepository.Setup(x => x.GetAsync(
            u => u.Email == email,
            null,
            false,
            true,
            default))
            .ReturnsAsync(new User { Status = true });

        // Act
        Func<Task> act = async () => await _businessRules.UserAlreadyVerified(email);

        // Assert
        await act.Should().ThrowAsync<BusinessException>().WithMessage(AuthMessages.UserAlreadyVerified);
    }

    [Fact]
    public void VerificationCodeShouldBeValid_ShouldThrowWhenCodeIsIncorrect()
    {
        // Act
        Action act = () => _businessRules.VerificationCodeShouldBeValid("123456", "654321");

        // Assert
        act.Should().Throw<BusinessException>().WithMessage(AuthMessages.VerificationCodeIsIncorrectOrExpired);
    }

    [Fact]
    public void UserShouldBeVerifiedEmail_ShouldThrowWhenStatusIsFalse()
    {
        // Arrange
        var user = new User { Status = false };

        // Act
        Action act = () => _businessRules.UserShouldBeVerifiedEmail(user);

        // Assert
        act.Should().Throw<BusinessException>().WithMessage(AuthMessages.UserShouldBeVerifiedEmail);
    }

    [Fact]
    public void RefreshTokenShouldBeActive_ShouldThrowWhenTokenIsNull()
    {
        // Act
        Action act = () => _businessRules.RefreshTokenShouldBeActive(null);

        // Assert
        act.Should().Throw<AuthenticationException>().WithMessage(AuthMessages.InvalidRefreshToken);
    }

    [Fact]
    public void RefreshTokenShouldBeActive_ShouldThrowWhenTokenIsNotActive()
    {
        // Arrange
        var token = new RefreshToken
        {
            Revoked = DateTime.UtcNow // Makes IsActive false
        };

        // Act
        Action act = () => _businessRules.RefreshTokenShouldBeActive(token);

        // Assert
        act.Should().Throw<AuthenticationException>().WithMessage(AuthMessages.InvalidRefreshToken);
    }

    [Fact]
    public void PasswordAndEmailShouldBeValid_ShouldThrowWhenPasswordIsIncorrect()
    {
        // Arrange
        HashingHelper.CreatePasswordHash("correctPassword", out byte[] hash, out byte[] salt);

        // Act
        Action act = () => _businessRules.PasswordAndEmailShouldBeValid("wrongPassword", hash, salt);

        // Assert
        act.Should().Throw<BusinessException>().WithMessage(AuthMessages.PasswordOrEmailNotCorrect);
    }

    [Fact]
    public void PasswordAndEmailShouldBeValid_ShouldNotThrowWhenPasswordIsCorrect()
    {
        // Arrange
        HashingHelper.CreatePasswordHash("correctPassword", out byte[] hash, out byte[] salt);

        // Act
        Action act = () => _businessRules.PasswordAndEmailShouldBeValid("correctPassword", hash, salt);

        // Assert
        act.Should().NotThrow<BusinessException>();
    }

    [Fact]
    public void UserPasswordShouldBeMatch_ShouldThrowWhenPasswordIsIncorrect()
    {
        // Arrange
        HashingHelper.CreatePasswordHash("correctPassword", out byte[] hash, out byte[] salt);
        var user = new User { PasswordHash = hash, PasswordSalt = salt };

        // Act
        Action act = () => _businessRules.UserPasswordShouldBeMatch(user, "wrongPassword");

        // Assert
        act.Should().Throw<BusinessException>().WithMessage("Mevcut şifreniz hatalı.");
    }

    [Fact]
    public void UserPasswordShouldBeMatch_ShouldNotThrowWhenPasswordIsCorrect()
    {
        // Arrange
        HashingHelper.CreatePasswordHash("correctPassword", out byte[] hash, out byte[] salt);
        var user = new User { PasswordHash = hash, PasswordSalt = salt };

        // Act
        Action act = () => _businessRules.UserPasswordShouldBeMatch(user, "correctPassword");

        // Assert
        act.Should().NotThrow<BusinessException>();
    }
}
