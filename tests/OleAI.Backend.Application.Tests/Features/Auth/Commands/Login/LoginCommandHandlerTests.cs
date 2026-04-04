using System.Security.Claims;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Auth.Commands.Login;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Features.Auth.Dtos.Requests;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Core.Helpers;
using OleAI.Backend.Core.Security.Hashing;
using OleAI.Backend.Core.Security.Jwt;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Auth.Commands.Login;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IRefreshTokenRepository> _mockRefreshTokenRepo;
    private readonly Mock<ITokenHelper> _mockTokenHelper;
    private readonly AuthBusinessRules _authBusinessRules;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockRefreshTokenRepo = new Mock<IRefreshTokenRepository>();
        _mockTokenHelper = new Mock<ITokenHelper>();
        _authBusinessRules = new AuthBusinessRules(_mockUserRepository.Object);

        _handler = new LoginCommandHandler(
            _mockUserRepository.Object,
            _mockRefreshTokenRepo.Object,
            _authBusinessRules,
            _mockTokenHelper.Object
        );
    }

    [Fact]
    public async Task Handle_UserNotFound_ShouldThrowBusinessException_WithGenericMessage()
    {
        // Arrange — user does not exist
        var command = new LoginCommand
        {
            Request = new LoginRequest { Email = "ghost@test.com", Password = "123" },
            IpAddress = "127.0.0.1"
        };

        _mockUserRepository.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()
            , It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>()
            , false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert — must use the unified InvalidCredentials message (not the old "UserNotFound")
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage(AuthMessages.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_WrongPassword_ShouldThrowBusinessException_WithSameGenericMessage()
    {
        // Arrange — user exists but password is wrong
        HashingHelper.CreatePasswordHash("correctPassword", out byte[] hash, out byte[] salt);
        var command = new LoginCommand
        {
            Request = new LoginRequest { Email = "test@test.com", Password = "wrongPassword" },
            IpAddress = "127.0.0.1"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            PasswordHash = hash,
            PasswordSalt = salt,
            Status = true,
            UserOperationClaims = new List<UserOperationClaim>()
        };

        _mockUserRepository.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()
            , It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>()
            , false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert — same generic message as user-not-found (anti-enumeration)
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage(AuthMessages.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_UserNotFound_And_WrongPassword_ShouldReturn_IdenticalErrorMessages()
    {
        // Scenario 1: Non-existent user
        var commandNoUser = new LoginCommand
        {
            Request = new LoginRequest { Email = "ghost@test.com", Password = "123" },
            IpAddress = "127.0.0.1"
        };

        _mockUserRepository.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()
            , It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>()
            , false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        string? messageNoUser = null;
        try { await _handler.Handle(commandNoUser, CancellationToken.None); }
        catch (BusinessException ex) { messageNoUser = ex.Message; }

        // Scenario 2: Existing user, wrong password
        HashingHelper.CreatePasswordHash("correctPassword", out byte[] hash, out byte[] salt);
        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            PasswordHash = hash,
            PasswordSalt = salt,
            Status = true,
            UserOperationClaims = new List<UserOperationClaim>()
        };

        var commandWrongPwd = new LoginCommand
        {
            Request = new LoginRequest { Email = "test@test.com", Password = "wrongPassword" },
            IpAddress = "127.0.0.1"
        };

        _mockUserRepository.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()
            , It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>()
            , false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        string? messageWrongPwd = null;
        try { await _handler.Handle(commandWrongPwd, CancellationToken.None); }
        catch (BusinessException ex) { messageWrongPwd = ex.Message; }

        // The core anti-enumeration assertion: both messages MUST be identical
        messageNoUser.Should().Be(messageWrongPwd);
        messageNoUser.Should().Be(AuthMessages.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_UserEmailNotVerified_ShouldThrowBusinessException()
    {
        // Arrange
        HashingHelper.CreatePasswordHash("123", out byte[] hash, out byte[] salt);
        var command = new LoginCommand
        {
            Request = new LoginRequest { Email = "unverified@test.com", Password = "123" },
            IpAddress = "127.0.0.1"
        };

        var unverifiedUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "unverified@test.com",
            PasswordHash = hash,
            PasswordSalt = salt,
            Status = false, // email not verified
            UserOperationClaims = new List<UserOperationClaim>()
        };

        _mockUserRepository.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()
            , It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>()
            , false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(unverifiedUser);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage(AuthMessages.UserShouldBeVerifiedEmail);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldReturnLoggedResponse()
    {
        // Arrange
        var request = new LoginRequest { Email = "test@test.com", Password = "123" };
        var command = new LoginCommand { Request = request, IpAddress = "127.0.0.1" };

        HashingHelper.CreatePasswordHash("123", out byte[] hash, out byte[] salt);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            PasswordHash = hash,
            PasswordSalt = salt,
            Status = true,
            UserOperationClaims = new List<UserOperationClaim>
            {
                new UserOperationClaim { operationClaim = new OperationClaim { Name = "Admin" } }
            }
        };

        var accessToken = new AccessToken { Token = "access", Expriration = DateTime.UtcNow.AddMinutes(15) };
        var refreshTokenCore = new OleAI.Backend.Core.Security.Jwt.RefreshToken { Token = "refresh", Expiration = DateTime.UtcNow.AddDays(7) };

        _mockUserRepository.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
            It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>(),
            false,
            true,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mockTokenHelper.Setup(t => t.CreateToken(It.IsAny<IEnumerable<Claim>>()))
            .Returns(accessToken);

        _mockTokenHelper.Setup(t => t.CreateRefreshToken())
            .Returns(refreshTokenCore);

        _mockRefreshTokenRepo.Setup(r => r.AddAsync(It.IsAny<OleAI.Backend.Domain.Entities.RefreshToken>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OleAI.Backend.Domain.Entities.RefreshToken());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be(accessToken);
        result.RefreshToken.Should().Be(refreshTokenCore);

        _mockRefreshTokenRepo.Verify(r => r.AddAsync(It.Is<OleAI.Backend.Domain.Entities.RefreshToken>(rt => rt.Token == "refresh"), It.IsAny<CancellationToken>()), Times.Once);
    }
}
