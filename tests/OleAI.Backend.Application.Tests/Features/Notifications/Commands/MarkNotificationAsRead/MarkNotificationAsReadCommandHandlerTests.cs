using System.Linq.Expressions;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Notifications.Commands.MarkNotificationAsRead;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Notifications.Commands.MarkNotificationAsRead;

public class MarkNotificationAsReadCommandHandlerTests
{
    private readonly Mock<INotificationRepository> _notificationRepositoryMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly MarkNotificationAsReadCommandHandler _handler;

    public MarkNotificationAsReadCommandHandlerTests()
    {
        _notificationRepositoryMock = new Mock<INotificationRepository>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _handler = new MarkNotificationAsReadCommandHandler(_notificationRepositoryMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUserNotAuthenticated_ShouldReturnFalse()
    {
        // Arrange
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);
        var command = new MarkNotificationAsReadCommand { NotificationId = Guid.NewGuid() };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenNotificationNotFound_ShouldReturnFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(userId);
        var command = new MarkNotificationAsReadCommand { NotificationId = Guid.NewGuid() };

        _notificationRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Notification, bool>>>(), It.IsAny<Func<IQueryable<Notification>, IIncludableQueryable<Notification, object>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Notification?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenNotificationIsAlreadyRead_ShouldNotUpdateAndReturnTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(userId);
        var command = new MarkNotificationAsReadCommand { NotificationId = Guid.NewGuid() };

        var notification = new Notification(Guid.NewGuid(), userId, "Title", "Message", NotificationType.System);
        notification.MarkAsRead(); // Set IsRead = true

        _notificationRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Notification, bool>>>(), It.IsAny<Func<IQueryable<Notification>, IIncludableQueryable<Notification, object>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _notificationRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenNotificationIsUnread_ShouldMarkAsReadAndReturnTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(userId);
        var command = new MarkNotificationAsReadCommand { NotificationId = Guid.NewGuid() };

        var notification = new Notification(Guid.NewGuid(), userId, "Title", "Message", NotificationType.System);
        // By default IsRead is false

        _notificationRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Notification, bool>>>(), It.IsAny<Func<IQueryable<Notification>, IIncludableQueryable<Notification, object>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        notification.IsRead.Should().BeTrue();
        _notificationRepositoryMock.Verify(x => x.UpdateAsync(notification, It.IsAny<CancellationToken>()), Times.Once);
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
