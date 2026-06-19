using System.Security.Claims;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using OleAI.Backend.Application.Features.Notifications.Dtos;
using OleAI.Backend.Application.Features.Notifications.Queries.GetMyNotifications;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Notifications.Queries.GetMyNotifications;

public class GetMyNotificationsQueryHandlerTests
{
    private readonly Mock<INotificationRepository> _notificationRepositoryMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetMyNotificationsQueryHandler _handler;

    public GetMyNotificationsQueryHandlerTests()
    {
        _notificationRepositoryMock = new Mock<INotificationRepository>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetMyNotificationsQueryHandler(_notificationRepositoryMock.Object, _httpContextAccessorMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUserNotAuthenticated_ShouldReturnEmptyPaginate()
    {
        // Arrange
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);
        var query = new GetMyNotificationsQuery { Page = 0, Size = 10, OnlyUnread = false };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ShouldReturnMappedPaginate()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(userId);
        var query = new GetMyNotificationsQuery { Page = 0, Size = 10, OnlyUnread = false };

        var notifications = new Paginate<Notification>();
        var dtoPaginate = new Paginate<NotificationDto>();

        _notificationRepositoryMock.Setup(x => x.GetListAsync(It.IsAny<QueryOptions<Notification>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);

        _mapperMock.Setup(x => x.Map<Paginate<NotificationDto>>(notifications)).Returns(dtoPaginate);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().Be(dtoPaginate);
        _notificationRepositoryMock.Verify(x => x.GetListAsync(It.IsAny<QueryOptions<Notification>>(), It.IsAny<CancellationToken>()), Times.Once);
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
