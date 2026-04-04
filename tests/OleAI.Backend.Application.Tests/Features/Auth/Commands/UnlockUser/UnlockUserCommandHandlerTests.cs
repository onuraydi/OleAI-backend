using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Auth.Commands.UnlockUser;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Features.Auth.Dtos.Requests;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Auth.Commands.UnlockUser;

public class UnlockUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly UnlockUserCommandHandler _handler;

    public UnlockUserCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new UnlockUserCommandHandler(_mockUserRepository.Object);
    }

    [Fact]
    public async Task Handle_UserNotFound_ShouldThrowBusinessException()
    {
        // Arrange
        var request = new UnlockUserRequest { UserId = Guid.NewGuid() };
        var command = new UnlockUserCommand { Request = request };

        _mockUserRepository.Setup(r => r.GetAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>(),
            false,
            true,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage(AuthMessages.TargetUserNotFound);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldUnlockUser_AndReturnSuccessResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new UnlockUserRequest { UserId = userId };
        var command = new UnlockUserCommand { Request = request };

        var user = new User
        {
            Id = userId,
            Email = "locked@test.com",
            FailedLoginAttempts = 25,
            LockoutEnd = DateTime.UtcNow.AddDays(10),
            IsActive = false
        };

        _mockUserRepository.Setup(r => r.GetAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>(),
            false,
            true,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        user.IsActive.Should().BeTrue();
        user.FailedLoginAttempts.Should().Be(0);
        user.LockoutEnd.Should().BeNull();

        result.Should().NotBeNull();
        result.Message.Should().Be("Kullanıcı hesabı kilidi başarıyla kaldırıldı.");

        _mockUserRepository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }
}
