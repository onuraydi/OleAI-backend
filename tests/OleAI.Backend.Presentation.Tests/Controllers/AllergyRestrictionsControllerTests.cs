using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OleAI.Backend.Application.Features.AllergyRestrictions.Commands.CreateAllergyRestriction;
using OleAI.Backend.Application.Features.AllergyRestrictions.Commands.DeleteAllergyRestriction;
using OleAI.Backend.Application.Features.AllergyRestrictions.Commands.UpdateAllergyRestriction;
using OleAI.Backend.Application.Features.AllergyRestrictions.Dtos.Responses;
using OleAI.Backend.Application.Features.AllergyRestrictions.Queries.GetAllergyRestrictionById;
using OleAI.Backend.Application.Features.AllergyRestrictions.Queries.GetAllergyRestrictionList;
using OleAI.Backend.Presentation.Controllers;
using Xunit;

namespace OleAI.Backend.Presentation.Tests.Controllers;

public class AllergyRestrictionsControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly AllergyRestrictionsController _controller;

    public AllergyRestrictionsControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new AllergyRestrictionsController();

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
        var response = new List<AllergyRestrictionListResponse>();
        _mockMediator.Setup(m => m.Send(It.IsAny<GetAllergyRestrictionListQuery>(), default)).ReturnsAsync(response);

        var result = await _controller.GetList();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_ShouldSendQuery_AndReturnOk()
    {
        var response = new AllergyRestrictionDetailResponse();
        _mockMediator.Setup(m => m.Send(It.IsAny<GetAllergyRestrictionByIdQuery>(), default)).ReturnsAsync(response);

        var result = await _controller.GetById(Guid.NewGuid());

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
    }

    [Fact]
    public async Task Add_ShouldSendCommand_AndReturnCreated()
    {
        var command = new CreateAllergyRestrictionCommand();
        _mockMediator.Setup(m => m.Send(It.IsAny<CreateAllergyRestrictionCommand>(), default)).ReturnsAsync("created");

        var result = await _controller.Add(command);

        var createdResult = result as CreatedResult;
        createdResult.Should().NotBeNull();
    }

    [Fact]
    public async Task Update_ShouldSendCommand_AndReturnOk()
    {
        var command = new UpdateAllergyRestrictionCommand { Id = Guid.NewGuid() };
        _mockMediator.Setup(m => m.Send(It.IsAny<UpdateAllergyRestrictionCommand>(), default)).ReturnsAsync("updated");

        var result = await _controller.Update(command);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        _mockMediator.Verify(m => m.Send(command, default), Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldSendCommand_AndReturnOk()
    {
        _mockMediator.Setup(m => m.Send(It.IsAny<DeleteAllergyRestrictionCommand>(), default)).ReturnsAsync("deleted");

        var result = await _controller.Delete(Guid.NewGuid());

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
    }
}
