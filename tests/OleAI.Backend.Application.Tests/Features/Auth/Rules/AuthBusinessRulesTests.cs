using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
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
            .ReturnsAsync(new User { Status = true });

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
        var act = () => _rules.UserShouldBeVerifiedEmail(new User { Status = false });
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
