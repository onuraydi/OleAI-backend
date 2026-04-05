using System.Security.Claims;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OleAI.Backend.Application.Features.Pantry.Commands.AddUserIngredient;
using OleAI.Backend.Application.Features.Pantry.Commands.CancelRequest;
using OleAI.Backend.Application.Features.Pantry.Commands.ClearExpiredUserIngredients;
using OleAI.Backend.Application.Features.Pantry.Commands.DeleteUserIngredient;
using OleAI.Backend.Application.Features.Pantry.Commands.UpdateUserIngredient;
using OleAI.Backend.Application.Features.Pantry.Dtos.Requests;
using OleAI.Backend.Application.Features.Pantry.Dtos.Responses;
using OleAI.Backend.Application.Features.Pantry.Queries.GetMyPantry;
using OleAI.Backend.Application.Features.Pantry.Queries.SearchIngredient;
using OleAI.Backend.Presentation.Controllers;
using Xunit;

namespace OleAI.Backend.Presentation.Tests.Controllers;

public class PantryControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly PantryController _controller;
    private readonly Guid _userId;

    public PantryControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new PantryController();
        _userId = Guid.NewGuid();

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(x => x.GetService(typeof(IMediator))).Returns(_mockMediator.Object);

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, _userId.ToString())
        }));

        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider.Object,
            User = claimsPrincipal
        };

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task GetMyPantry_ShouldSendQueryWithLoggedInUserId_AndReturnOk()
    {
        // Arrange
        var response = new List<PantryItemResponse> { new() { Id = Guid.NewGuid(), Quantity = 2 } };

        _mockMediator.Setup(m => m.Send(It.IsAny<GetMyPantryQuery>(), default))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetMyPantry();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().Be(response);

        _mockMediator.Verify(m => m.Send(It.Is<GetMyPantryQuery>(q => q.UserId == _userId), default), Times.Once);
    }

    [Fact]
    public async Task SearchIngredients_ShouldSendKeyword_AndReturnOk()
    {
        // Arrange
        var response = new List<IngredientSearchResultResponse>();

        _mockMediator.Setup(m => m.Send(It.IsAny<SearchIngredientQuery>(), default))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.SearchIngredients("su");

        // Assert
        (result as OkObjectResult).Should().NotBeNull();
        _mockMediator.Verify(m => m.Send(It.Is<SearchIngredientQuery>(q => q.Keyword == "su"), default), Times.Once);
    }

    [Fact]
    public async Task AddIngredient_ShouldOverrideUserIdFromToken_AndReturnCreated()
    {
        // Arrange
        var command = new AddUserIngredientCommand
        {
            Request = new AddUserIngredientRequest { UserId = Guid.NewGuid(), RequestedName = "Yeni", Quantity = 1 }
        };

        var response = new AddedUserIngredientResponse { Message = "ok", IsPendingApproval = true };

        _mockMediator.Setup(m => m.Send(It.IsAny<AddUserIngredientCommand>(), default))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.AddIngredient(command);

        // Assert
        var createdResult = result as CreatedResult;
        createdResult.Should().NotBeNull();
        createdResult!.Value.Should().Be(response);

        _mockMediator.Verify(m => m.Send(It.Is<AddUserIngredientCommand>(c => c.Request.UserId == _userId), default), Times.Once);
    }

    [Fact]
    public async Task UpdateIngredientQuantity_ShouldSendCommandWithRouteAndUser_AndReturnOk()
    {
        // Arrange
        var inventoryId = Guid.NewGuid();
        _mockMediator.Setup(m => m.Send(It.IsAny<UpdateUserIngredientCommand>(), default))
            .ReturnsAsync("updated");

        // Act
        var result = await _controller.UpdateIngredientQuantity(inventoryId, 4.5);

        // Assert
        (result as OkObjectResult).Should().NotBeNull();

        _mockMediator.Verify(m => m.Send(
            It.Is<UpdateUserIngredientCommand>(c =>
                c.Request.Id == inventoryId &&
                c.Request.UserId == _userId &&
                c.Request.NewQuantity == 4.5),
            default), Times.Once);
    }

    [Fact]
    public async Task DeleteIngredient_ShouldSendCommandWithRouteAndUser_AndReturnOk()
    {
        // Arrange
        var inventoryId = Guid.NewGuid();
        _mockMediator.Setup(m => m.Send(It.IsAny<DeleteUserIngredientCommand>(), default))
            .ReturnsAsync("deleted");

        // Act
        var result = await _controller.DeleteIngredient(inventoryId);

        // Assert
        (result as OkObjectResult).Should().NotBeNull();
        _mockMediator.Verify(m => m.Send(It.Is<DeleteUserIngredientCommand>(c => c.Id == inventoryId && c.UserId == _userId), default), Times.Once);
    }

    [Fact]
    public async Task ClearExpiredIngredients_ShouldSendCommandWithUser_AndReturnOk()
    {
        // Arrange
        _mockMediator.Setup(m => m.Send(It.IsAny<ClearExpiredUserIngredientsCommand>(), default))
            .ReturnsAsync("cleaned");

        // Act
        var result = await _controller.ClearExpiredIngredients();

        // Assert
        (result as OkObjectResult).Should().NotBeNull();
        _mockMediator.Verify(m => m.Send(It.Is<ClearExpiredUserIngredientsCommand>(c => c.userId == _userId), default), Times.Once);
    }

    [Fact]
    public async Task CancelPendingRequest_ShouldSendCommandWithRequestIdAndUser_AndReturnOk()
    {
        // Arrange
        var requestId = Guid.NewGuid();
        _mockMediator.Setup(m => m.Send(It.IsAny<CancelIngredientRequestCommand>(), default))
            .ReturnsAsync("cancelled");

        // Act
        var result = await _controller.CancelPendingRequest(requestId);

        // Assert
        (result as OkObjectResult).Should().NotBeNull();
        _mockMediator.Verify(m => m.Send(It.Is<CancelIngredientRequestCommand>(c => c.RequestId == requestId && c.UserId == _userId), default), Times.Once);
    }
}
