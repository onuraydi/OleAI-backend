using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OleAI.Backend.Application.Features.Pantry.Commands.ResolveRequest;
using OleAI.Backend.Application.Features.Pantry.Dtos.Requests;
using OleAI.Backend.Application.Features.Pantry.Dtos.Responses;
using OleAI.Backend.Application.Features.Pantry.Queries.GetPendingRequests;
using OleAI.Backend.Application.Features.Pantry.Queries.GetResolvedRequests;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Enums;
using OleAI.Backend.Presentation.Controllers;
using Xunit;

namespace OleAI.Backend.Presentation.Tests.Controllers;

public class AdminPantryControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly AdminPantryController _controller;

    public AdminPantryControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new AdminPantryController();

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(x => x.GetService(typeof(IMediator))).Returns(_mockMediator.Object);

        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider.Object
        };

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task GetPendingRequests_ShouldSendQuery_AndReturnOk()
    {
        // Arrange
        var response = new List<PendingRequestDto>();

        _mockMediator.Setup(m => m.Send(It.IsAny<GetPendingIngredientRequestsQuery>(), default))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetPendingRequests();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().Be(response);

        _mockMediator.Verify(m => m.Send(It.IsAny<GetPendingIngredientRequestsQuery>(), default), Times.Once);
    }

    [Fact]
    public async Task ResolveRequest_ShouldSendCommand_AndReturnOk()
    {
        // Arrange
        var command = new ResolveIngredientRequestCommand
        {
            Request = new ResolveIngredientRequestDto
            {
                RequestId = Guid.NewGuid(),
                Action = ResolutionAction.Reject,
                AdminNote = "invalid"
            }
        };

        _mockMediator.Setup(m => m.Send(command, default))
            .ReturnsAsync("resolved");

        // Act
        var result = await _controller.ResolveRequest(command);

        // Assert
        (result as OkObjectResult).Should().NotBeNull();
        _mockMediator.Verify(m => m.Send(command, default), Times.Once);
    }

    [Fact]
    public async Task GetResolvedRequests_ShouldSendGivenQuery_AndReturnOk()
    {
        // Arrange
        var query = new GetResolvedIngredientRequestsQuery { Page = 1, Size = 10 };
        var response = new Paginate<ResolvedRequestsResponse>
        {
            Items = new List<ResolvedRequestsResponse>(),
            PageNumber = 1,
            PageSize = 10,
            TotalItems = 0,
            TotalPages = 0
        };

        _mockMediator.Setup(m => m.Send(query, default))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetResolvedRequests(query);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().Be(response);

        _mockMediator.Verify(m => m.Send(query, default), Times.Once);
    }
}
