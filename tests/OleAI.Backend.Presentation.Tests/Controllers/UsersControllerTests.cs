using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OleAI.Backend.Application.Features.Users.Commands.UpdateUserDislikedIngredients;
using OleAI.Backend.Presentation.Controllers;
using Xunit;

namespace OleAI.Backend.Presentation.Tests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new UsersController();

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
    public async Task UpdateDislikedIngredients_ShouldSendCommand_AndReturnOk()
    {
        var command = new UpdateUserDislikedIngredientsCommand
        {
            IngredientIds = new List<Guid> { Guid.NewGuid() }
        };

        _mockMediator.Setup(m => m.Send(It.IsAny<UpdateUserDislikedIngredientsCommand>(), default))
            .ReturnsAsync("Sevilmeyen malzemeleriniz başarıyla güncellendi.");

        var result = await _controller.UpdateDislikedIngredients(command);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();

        // Object result validation
        var value = okResult!.Value;
        var messageProp = value?.GetType().GetProperty("Message")?.GetValue(value);
        messageProp.Should().Be("Sevilmeyen malzemeleriniz başarıyla güncellendi.");

        _mockMediator.Verify(m => m.Send(command, default), Times.Once);
    }
}
