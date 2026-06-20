using System;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OleAI.Backend.Application.Features.ContactMessages.Commands.CreateContactMessage;
using OleAI.Backend.Presentation.Controllers;
using Xunit;

namespace OleAI.Backend.Presentation.Tests.Controllers
{
    public class ContactMessagesControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly ContactMessagesController _controller;

        public ContactMessagesControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new ContactMessagesController();

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
        public async Task Add_ShouldSendCommand_AndReturnCreated()
        {
            var command = new CreateContactMessageCommand();
            var response = new OleAI.Backend.Application.Features.ContactMessages.Commands.CreateContactMessage.CreateContactMessageResponse();
            _mockMediator.Setup(m => m.Send(It.IsAny<CreateContactMessageCommand>(), default)).ReturnsAsync(response);

            var result = await _controller.Add(command);

            var createdResult = result as CreatedResult;
            createdResult.Should().NotBeNull();
            createdResult!.Value.Should().Be(response);
        }
    }
}
