using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OleAI.Backend.Application.Features.ContactMessages.Commands.DeleteContactMessage;
using OleAI.Backend.Application.Features.ContactMessages.Commands.MarkMessageAsRead;
using OleAI.Backend.Application.Features.ContactMessages.Queries.GetContactMessages;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Presentation.Controllers;
using Xunit;

namespace OleAI.Backend.Presentation.Tests.Controllers
{
    public class AdminContactMessagesControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly AdminContactMessagesController _controller;

        public AdminContactMessagesControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new AdminContactMessagesController();

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
        public async Task GetList_ShouldSendQuery_AndReturnOk()
        {
            var response = new Paginate<OleAI.Backend.Application.Features.ContactMessages.Queries.GetContactMessages.GetContactMessageListItemDto> { Items = new List<OleAI.Backend.Application.Features.ContactMessages.Queries.GetContactMessages.GetContactMessageListItemDto>() };
            _mockMediator.Setup(m => m.Send(It.IsAny<GetContactMessagesQuery>(), default)).ReturnsAsync(response);

            var result = await _controller.GetList(0, 10);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(response);
        }

        [Fact]
        public async Task MarkAsRead_ShouldSendCommand_AndReturnOk()
        {
            var id = Guid.NewGuid();
            _mockMediator.Setup(m => m.Send(It.IsAny<MarkContactMessageAsReadCommand>(), default)).ReturnsAsync(true);

            var result = await _controller.MarkAsRead(id);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldSendCommand_AndReturnOk()
        {
            var id = Guid.NewGuid();
            var response = new OleAI.Backend.Application.Features.ContactMessages.Commands.DeleteContactMessage.DeletedContactMessageResponse();
            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteContactMessageCommand>(), default)).ReturnsAsync(response);

            var result = await _controller.Delete(id);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(response);
        }
    }
}
