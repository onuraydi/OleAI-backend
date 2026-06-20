using System;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OleAI.Backend.Application.Features.Admin.Queries.GetDashboardStats;
using OleAI.Backend.Presentation.Controllers;
using Xunit;


namespace OleAI.Backend.Presentation.Tests.Controllers
{
    public class AdminDashboardControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly AdminDashboardController _controller;

        public AdminDashboardControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new AdminDashboardController();

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

        [Fact]
        public async Task GetStats_ShouldSendQuery_AndReturnOk()
        {
            var response = new OleAI.Backend.Application.Features.Admin.Queries.GetDashboardStats.GetDashboardStatsResponse();
            _mockMediator.Setup(m => m.Send(It.IsAny<GetDashboardStatsQuery>(), default)).ReturnsAsync(response);

            var result = await _controller.GetStats();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(response);
        }
    }
}
