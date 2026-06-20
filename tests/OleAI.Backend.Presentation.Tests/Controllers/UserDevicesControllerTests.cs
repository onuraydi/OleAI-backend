using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OleAI.Backend.Application.Features.Notifications.Commands.RegisterDevice;
using OleAI.Backend.Presentation.Controllers;
using Xunit;

namespace OleAI.Backend.Presentation.Tests.Controllers
{
    public class UserDevicesControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly UserDevicesController _controller;

        public UserDevicesControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new UserDevicesController(_mockMediator.Object);
        }

        [Fact]
        public async Task RegisterDevice_ShouldSendCommand_AndReturnOk()
        {
            var command = new RegisterDeviceCommand();
            var response = true;
            _mockMediator.Setup(m => m.Send(It.IsAny<RegisterDeviceCommand>(), default)).ReturnsAsync(response);

            var result = await _controller.RegisterDevice(command);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
        }
    }
}
