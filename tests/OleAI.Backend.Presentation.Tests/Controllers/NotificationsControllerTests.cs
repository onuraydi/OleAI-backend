using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OleAI.Backend.Application.Features.Notifications.Commands.DeleteNotification;
using OleAI.Backend.Application.Features.Notifications.Commands.MarkAllNotificationsAsRead;
using OleAI.Backend.Application.Features.Notifications.Commands.MarkNotificationAsRead;
using OleAI.Backend.Application.Features.Notifications.Queries.GetMyNotifications;
using OleAI.Backend.Application.Features.Notifications.Queries.GetUnreadNotificationCount;
using OleAI.Backend.Application.Services.PushNotifications;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Presentation.Controllers;
using Xunit;

namespace OleAI.Backend.Presentation.Tests.Controllers
{
    public class NotificationsControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly NotificationsController _controller;
        private readonly Mock<INotificationRepository> _mockNotificationRepo;
        private readonly Mock<IUserDeviceRepository> _mockUserDeviceRepo;
        private readonly Mock<IPushNotificationService> _mockPushNotificationService;

        public NotificationsControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new NotificationsController();

            _mockNotificationRepo = new Mock<INotificationRepository>();
            _mockUserDeviceRepo = new Mock<IUserDeviceRepository>();
            _mockPushNotificationService = new Mock<IPushNotificationService>();

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator))).Returns(_mockMediator.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = serviceProvider.Object
                }
            };
        }

        private void SetupUserIdentity(string userId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
            }, "mock"));

            _controller.ControllerContext.HttpContext.User = user;
        }

        [Fact]
        public async Task GetMyNotifications_ShouldReturnOk()
        {
            var response = new Paginate<OleAI.Backend.Application.Features.Notifications.Dtos.NotificationDto> { Items = new List<OleAI.Backend.Application.Features.Notifications.Dtos.NotificationDto>() };
            _mockMediator.Setup(m => m.Send(It.IsAny<GetMyNotificationsQuery>(), default)).ReturnsAsync(response);

            var result = await _controller.GetMyNotifications(0, 10, false);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(response);
        }

        [Fact]
        public async Task GetUnreadCount_ShouldReturnOk()
        {
            int count = 5;
            _mockMediator.Setup(m => m.Send(It.IsAny<GetUnreadNotificationCountQuery>(), default)).ReturnsAsync(count);

            var result = await _controller.GetUnreadCount();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
        }

        [Fact]
        public async Task MarkAsRead_WhenSuccess_ShouldReturnOk()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<MarkNotificationAsReadCommand>(), default)).ReturnsAsync(true);

            var result = await _controller.MarkAsRead(Guid.NewGuid());

            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task MarkAsRead_WhenFailure_ShouldReturnNotFound()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<MarkNotificationAsReadCommand>(), default)).ReturnsAsync(false);

            var result = await _controller.MarkAsRead(Guid.NewGuid());

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task MarkAllAsRead_ShouldReturnOk()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<MarkAllNotificationsAsReadCommand>(), default)).ReturnsAsync(true);

            var result = await _controller.MarkAllAsRead();

            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task Delete_WhenSuccess_ShouldReturnOk()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteNotificationCommand>(), default)).ReturnsAsync(true);

            var result = await _controller.Delete(Guid.NewGuid());

            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task Delete_WhenFailure_ShouldReturnNotFound()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteNotificationCommand>(), default)).ReturnsAsync(false);

            var result = await _controller.Delete(Guid.NewGuid());

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task TriggerTestNotification_WhenNoUser_ShouldReturnUnauthorized()
        {
            var result = await _controller.TriggerTestNotification(_mockNotificationRepo.Object, _mockUserDeviceRepo.Object, _mockPushNotificationService.Object);

            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task TriggerTestNotification_WhenHasUser_ShouldReturnOk()
        {
            var userId = Guid.NewGuid();
            SetupUserIdentity(userId.ToString());

            var deviceList = new Paginate<UserDevice>
            {
                Items = new List<UserDevice>
                {
                    new UserDevice(Guid.NewGuid(), userId, "token1", OleAI.Backend.Domain.Enums.DevicePlatform.Android)
                }
            };

            _mockUserDeviceRepo.Setup(x => x.GetListAsync(It.IsAny<QueryOptions<UserDevice>>(), default))
                               .ReturnsAsync(deviceList);

            var result = await _controller.TriggerTestNotification(_mockNotificationRepo.Object, _mockUserDeviceRepo.Object, _mockPushNotificationService.Object);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            _mockPushNotificationService.Verify(x => x.SendMulticastPushNotificationAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(), It.IsAny<string>(), null), Times.Once);
        }

        [Fact]
        public async Task SendGlobalNotification_WhenNoUser_ShouldReturnUnauthorized()
        {
            var result = await _controller.SendGlobalNotification("test msg", _mockNotificationRepo.Object, _mockUserDeviceRepo.Object, _mockPushNotificationService.Object);

            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task SendGlobalNotification_WhenHasUser_ShouldReturnOk()
        {
            var adminId = Guid.NewGuid();
            SetupUserIdentity(adminId.ToString());

            var deviceList = new Paginate<UserDevice>
            {
                Items = new List<UserDevice>
                {
                    new UserDevice(Guid.NewGuid(), Guid.NewGuid(), "token1", OleAI.Backend.Domain.Enums.DevicePlatform.Android),
                    new UserDevice(Guid.NewGuid(), Guid.NewGuid(), "token2", OleAI.Backend.Domain.Enums.DevicePlatform.IOS)
                }
            };

            _mockUserDeviceRepo.Setup(x => x.GetListAsync(It.IsAny<QueryOptions<UserDevice>>(), default))
                               .ReturnsAsync(deviceList);

            var result = await _controller.SendGlobalNotification("test msg", _mockNotificationRepo.Object, _mockUserDeviceRepo.Object, _mockPushNotificationService.Object);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            _mockPushNotificationService.Verify(x => x.SendMulticastPushNotificationAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(), It.IsAny<string>(), null), Times.Once);
        }
    }
}
