using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OleAI.Backend.Application.Features.DietaryPreferences.Commands.CreateDietaryPreference;
using OleAI.Backend.Application.Features.DietaryPreferences.Commands.DeleteDietaryPreference;
using OleAI.Backend.Application.Features.DietaryPreferences.Commands.UpdateDietaryPreference;
using OleAI.Backend.Application.Features.DietaryPreferences.Dtos.Responses;
using OleAI.Backend.Application.Features.DietaryPreferences.Queries.GetDietaryPreferenceById;
using OleAI.Backend.Application.Features.DietaryPreferences.Queries.GetDietaryPreferenceList;
using OleAI.Backend.Presentation.Controllers;
using Xunit;

namespace OleAI.Backend.Presentation.Tests.Controllers;

public class DietaryPreferencesControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly DietaryPreferencesController _controller;

    public DietaryPreferencesControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new DietaryPreferencesController();

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
        var response = new List<DietaryPreferenceListResponse>();
        _mockMediator.Setup(m => m.Send(It.IsAny<GetDietaryPreferenceListQuery>(), default)).ReturnsAsync(response);

        var result = await _controller.GetList();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().Be(response);
    }

    [Fact]
    public async Task GetById_ShouldSendQuery_AndReturnOk()
    {
        var id = Guid.NewGuid();
        var response = new DietaryPreferenceDetailResponse();
        _mockMediator.Setup(m => m.Send(It.IsAny<GetDietaryPreferenceByIdQuery>(), default)).ReturnsAsync(response);

        var result = await _controller.GetById(id);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().Be(response);
    }

    [Fact]
    public async Task Add_ShouldSendCommand_AndReturnCreated()
    {
        var command = new CreateDietaryPreferenceCommand();
        _mockMediator.Setup(m => m.Send(It.IsAny<CreateDietaryPreferenceCommand>(), default)).ReturnsAsync("created");

        var result = await _controller.Add(command);

        var createdResult = result as CreatedResult;
        createdResult.Should().NotBeNull();
    }

    [Fact]
    public async Task Update_ShouldSendCommand_AndReturnOk()
    {
        var command = new UpdateDietaryPreferenceCommand { Id = Guid.NewGuid() };
        _mockMediator.Setup(m => m.Send(It.IsAny<UpdateDietaryPreferenceCommand>(), default)).ReturnsAsync("updated");

        var result = await _controller.Update(command);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        _mockMediator.Verify(m => m.Send(command, default), Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldSendCommand_AndReturnOk()
    {
        var id = Guid.NewGuid();
        _mockMediator.Setup(m => m.Send(It.IsAny<DeleteDietaryPreferenceCommand>(), default)).ReturnsAsync("deleted");

        var result = await _controller.Delete(id);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
    }
}
