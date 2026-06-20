using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using OleAI.Backend.Application.Features.Notifications.Commands.MarkAllNotificationsAsRead;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Notifications.Commands.MarkAllNotificationsAsRead;

public class MarkAllNotificationsAsReadCommandHandlerTests
{
    private readonly Mock<INotificationRepository> _notificationRepositoryMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly MarkAllNotificationsAsReadCommandHandler _handler;

    public MarkAllNotificationsAsReadCommandHandlerTests()
    {
        _notificationRepositoryMock = new Mock<INotificationRepository>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _handler = new MarkAllNotificationsAsReadCommandHandler(_notificationRepositoryMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUserNotAuthenticated_ShouldReturnFalse()
    {
        // Arrange
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);
        var command = new MarkAllNotificationsAsReadCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenHasUnreadNotifications_ShouldMarkAsReadAndUpdate()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(userId);
        var command = new MarkAllNotificationsAsReadCommand();

        var notifications = new List<Notification>
        {
            new Notification(Guid.NewGuid(), userId, "Title1", "Msg1", NotificationType.System),
            new Notification(Guid.NewGuid(), userId, "Title2", "Msg2", NotificationType.System)
        };

        var paginatedResult = new Paginate<Notification> { Items = notifications };

        _notificationRepositoryMock.Setup(x => x.GetListAsync(It.IsAny<QueryOptions<Notification>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        notifications.Should().OnlyContain(n => n.IsRead == true);
        _notificationRepositoryMock.Verify(x => x.UpdateRangeAsync(notifications, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNoUnreadNotifications_ShouldReturnTrueButNotUpdate()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(userId);
        var command = new MarkAllNotificationsAsReadCommand();

        var paginatedResult = new Paginate<Notification> { Items = new List<Notification>() };

        _notificationRepositoryMock.Setup(x => x.GetListAsync(It.IsAny<QueryOptions<Notification>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _notificationRepositoryMock.Verify(x => x.UpdateRangeAsync(It.IsAny<List<Notification>>(), It.IsAny<CancellationToken>()), Times.Never);
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
