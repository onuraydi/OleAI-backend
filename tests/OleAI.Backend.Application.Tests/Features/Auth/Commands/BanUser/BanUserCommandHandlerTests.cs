using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Auth.Commands.BanUser;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Features.Auth.Dtos.Requests;
using OleAI.Backend.Application.Services.AuthService;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Auth.Commands.BanUser
{
    public class BanUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly BanUserCommandHandler _handler;

        public BanUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _authServiceMock = new Mock<IAuthService>();
            _handler = new BanUserCommandHandler(_userRepositoryMock.Object, _authServiceMock.Object);
        }

        [Fact]
        public async Task Handle_WhenUserExists_ShouldBanUserAndDeleteRefreshTokens()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new BanUserCommand
            {
                Request = new BanUserRequest { UserId = userId }
            };

            var user = new User
            {
                Id = userId,
                FirstName = "Test",
                LastName = "User",
                Email = "test@test.com",
                IsActive = true,
                PasswordHash = new byte[0],
                PasswordSalt = new byte[0]
            };

            _userRepositoryMock.Setup(x => x.GetAsync(
                It.IsAny<Expression<Func<User, bool>>>(),
                null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Message.Should().Be("Kullanıcı başarıyla banlandı.");
            user.IsActive.Should().BeFalse();

            _userRepositoryMock.Verify(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
            _authServiceMock.Verify(x => x.DeleteOldRefreshTokensAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUserDoesNotExist_ShouldThrowBusinessException()
        {
            // Arrange
            var command = new BanUserCommand
            {
                Request = new BanUserRequest { UserId = Guid.NewGuid() }
            };

            _userRepositoryMock.Setup(x => x.GetAsync(
                It.IsAny<Expression<Func<User, bool>>>(),
                null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage(AuthMessages.TargetUserNotFound);

            _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
            _authServiceMock.Verify(x => x.DeleteOldRefreshTokensAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
