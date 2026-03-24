using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Auth.Commands.ChangePassword;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Features.Auth.Dtos.Requests;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Core.Security.Hashing;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Auth.Commands.ChangePassword;

public class ChangePasswordCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly AuthBusinessRules _authBusinessRules;
    private readonly ChangePasswordCommandHandler _handler;

    public ChangePasswordCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _authBusinessRules = new AuthBusinessRules(_mockUserRepository.Object);

        _handler = new ChangePasswordCommandHandler(
            _mockUserRepository.Object,
            _authBusinessRules
        );
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldChangePassword()
    {
        // Arrange
        HashingHelper.CreatePasswordHash("oldPassword", out byte[] hash, out byte[] salt);

        var request = new ChangePasswordRequest { OldPassword = "oldPassword", NewPassword = "newPassword123" };
        var userId = Guid.NewGuid();
        var command = new ChangePasswordCommand { UserId = userId, Request = request };

        var user = new User { Id = userId, Email = "test@example.com", PasswordHash = hash, PasswordSalt = salt };

        _mockUserRepository.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
            It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>(),
            false,
            true,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Message.Should().Be(AuthMessages.PasswordChangedSuccessfully);

        // Ensure hash changed
        user.PasswordHash.Should().NotEqual(hash);
        user.PasswordSalt.Should().NotEqual(salt);

        _mockUserRepository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ShouldThrowBusinessException()
    {
        // Arrange
        var command = new ChangePasswordCommand
        {
            UserId = Guid.NewGuid(),
            Request = new ChangePasswordRequest { OldPassword = "old", NewPassword = "new" }
        };

        _mockUserRepository.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
            It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>(),
            false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage(AuthMessages.UserNotFound);
    }

    [Fact]
    public async Task Handle_WrongOldPassword_ShouldThrowBusinessException()
    {
        // Arrange
        HashingHelper.CreatePasswordHash("correctOld", out byte[] hash, out byte[] salt);
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, PasswordHash = hash, PasswordSalt = salt };

        var command = new ChangePasswordCommand
        {
            UserId = userId,
            Request = new ChangePasswordRequest { OldPassword = "wrongOld", NewPassword = "newPassword" }
        };

        _mockUserRepository.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
            It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>(),
            false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("Mevcut şifreniz hatalı.");
    }
}
