using System.Linq.Expressions;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using OleAI.Backend.Application.Features.Notifications.Queries.GetUnreadNotificationCount;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Notifications.Queries.GetUnreadNotificationCount;

public class GetUnreadNotificationCountQueryHandlerTests
{
    private readonly Mock<INotificationRepository> _notificationRepositoryMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly GetUnreadNotificationCountQueryHandler _handler;

    public GetUnreadNotificationCountQueryHandlerTests()
    {
        _notificationRepositoryMock = new Mock<INotificationRepository>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _handler = new GetUnreadNotificationCountQueryHandler(_notificationRepositoryMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUserNotAuthenticated_ShouldReturnZero()
    {
        // Arrange
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);
        var query = new GetUnreadNotificationCountQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ShouldReturnCount()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(userId);
        var query = new GetUnreadNotificationCountQuery();

        _notificationRepositoryMock.Setup(x => x.GetCountAsync(It.IsAny<Expression<Func<Notification, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().Be(5);
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
