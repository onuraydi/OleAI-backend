using System.Linq.Expressions;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Notifications.Commands.RegisterDevice;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Notifications.Commands.RegisterDevice;

public class RegisterDeviceCommandHandlerTests
{
    private readonly Mock<IUserDeviceRepository> _userDeviceRepositoryMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly RegisterDeviceCommandHandler _handler;

    public RegisterDeviceCommandHandlerTests()
    {
        _userDeviceRepositoryMock = new Mock<IUserDeviceRepository>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _handler = new RegisterDeviceCommandHandler(_userDeviceRepositoryMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUserNotAuthenticated_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);
        var command = new RegisterDeviceCommand { DeviceToken = "test-token", Platform = DevicePlatform.Android };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_WhenDeviceAlreadyExists_ShouldReturnTrueWithoutAdding()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(userId);
        var command = new RegisterDeviceCommand { DeviceToken = "test-token", Platform = DevicePlatform.Android };

        var existingDevice = new UserDevice(Guid.NewGuid(), userId, "test-token", DevicePlatform.Android);

        _userDeviceRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserDevice, bool>>>(), It.IsAny<Func<IQueryable<UserDevice>, IIncludableQueryable<UserDevice, object>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingDevice);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _userDeviceRepositoryMock.Verify(x => x.AddAsync(It.IsAny<UserDevice>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenNewDevice_ShouldAddAndReturnTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(userId);
        var command = new RegisterDeviceCommand { DeviceToken = "new-token", Platform = DevicePlatform.IOS };

        _userDeviceRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserDevice, bool>>>(), It.IsAny<Func<IQueryable<UserDevice>, IIncludableQueryable<UserDevice, object>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserDevice?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _userDeviceRepositoryMock.Verify(x => x.AddAsync(It.IsAny<UserDevice>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private void SetupUserContext(Guid userId)
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
    }
}
