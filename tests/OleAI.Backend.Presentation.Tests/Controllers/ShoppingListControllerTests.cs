using System.Security.Claims;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OleAI.Backend.Application.Features.ShoppingList.Commands.AddShoppingListItem;
using OleAI.Backend.Application.Features.ShoppingList.Commands.BulkMarkAsPurchased;
using OleAI.Backend.Application.Features.ShoppingList.Commands.MarkAsPurchased;
using OleAI.Backend.Application.Features.ShoppingList.Commands.RemoveShoppingListItem;
using OleAI.Backend.Application.Features.ShoppingList.Commands.UpdateShoppingListItem;
using OleAI.Backend.Application.Features.ShoppingList.Dtos.Requests;
using OleAI.Backend.Application.Features.ShoppingList.Dtos.Responses;
using OleAI.Backend.Application.Features.ShoppingList.Queries.GetMyShoppingList;
using OleAI.Backend.Domain.Enums;
using OleAI.Backend.Presentation.Controllers;

namespace OleAI.Backend.Presentation.Tests.Controllers;

public class ShoppingListControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly ShoppingListController _controller;
    private readonly Guid _userId;

    public ShoppingListControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new ShoppingListController();
        _userId = Guid.NewGuid();

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(x => x.GetService(typeof(IMediator))).Returns(_mockMediator.Object);

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, _userId.ToString())
        ]));

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
    public async Task GetMyShoppingList_ShouldSendQueryWithLoggedInUserId_AndReturnOk()
    {
        var response = new List<ShoppingListItemResponse> { new() { Id = Guid.NewGuid(), Quantity = 2, Unit = MeasurementUnit.Piece } };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetMyShoppingListQuery>(), default))
            .ReturnsAsync(response);

        var result = await _controller.GetMyShoppingList();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().Be(response);

        _mockMediator.Verify(m => m.Send(It.Is<GetMyShoppingListQuery>(q => q.UserId == _userId), default), Times.Once);
    }

    [Fact]
    public async Task AddItem_ShouldOverrideUserIdFromToken_AndReturnCreated()
    {
        var request = new AddShoppingListItemRequest
        {
            UserId = Guid.NewGuid(),
            IngredientId = Guid.NewGuid(),
            Quantity = 1.5,
            Unit = MeasurementUnit.Gram
        };

        var response = new ShoppingListItemResponse
        {
            Id = Guid.NewGuid(),
            IngredientId = request.IngredientId,
            IngredientName = "Un",
            Quantity = request.Quantity,
            Unit = request.Unit
        };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<AddShoppingListItemCommand>(), default))
            .ReturnsAsync(response);

        var result = await _controller.AddItem(request);

        var createdResult = result as CreatedResult;
        createdResult.Should().NotBeNull();
        createdResult!.Value.Should().Be(response);
        createdResult.Location.Should().Be($"api/shopping-list/{response.Id}");

        _mockMediator.Verify(m => m.Send(
            It.Is<AddShoppingListItemCommand>(c =>
                c.Request.UserId == _userId &&
                c.Request.IngredientId == request.IngredientId &&
                Math.Abs(c.Request.Quantity - request.Quantity) < 0.0001d &&
                c.Request.Unit == request.Unit),
            default), Times.Once);
    }

    [Fact]
    public async Task UpdateItem_ShouldSendCommandWithRouteAndUser_AndReturnOk()
    {
        var itemId = Guid.NewGuid();
        var request = new UpdateShoppingListItemRequest { NewQuantity = 3.25, NewUnit = MeasurementUnit.Piece };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<UpdateShoppingListItemCommand>(), default))
            .ReturnsAsync("updated");

        var result = await _controller.UpdateItem(itemId, request);

        (result as OkObjectResult).Should().NotBeNull();

        _mockMediator.Verify(m => m.Send(
            It.Is<UpdateShoppingListItemCommand>(c =>
                c.ItemId == itemId &&
                c.UserId == _userId &&
                Math.Abs(c.NewQuantity - request.NewQuantity) < 0.0001d &&
                c.NewUnit == request.NewUnit),
            default), Times.Once);
    }

    [Fact]
    public async Task MarkAsPurchased_ShouldSendCommandWithRouteAndUser_AndReturnOk()
    {
        var itemId = Guid.NewGuid();

        _mockMediator
            .Setup(m => m.Send(It.IsAny<MarkAsPurchasedCommand>(), default))
            .ReturnsAsync("ok");

        var result = await _controller.MarkAsPurchased(itemId);

        (result as OkObjectResult).Should().NotBeNull();

        _mockMediator.Verify(m => m.Send(
            It.Is<MarkAsPurchasedCommand>(c => c.ItemId == itemId && c.UserId == _userId),
            default), Times.Once);
    }

    [Fact]
    public async Task BulkMarkAsPurchased_ShouldSendCommandWithUserAndIds_AndReturnOk()
    {
        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<BulkMarkAsPurchasedCommand>(), default))
            .ReturnsAsync("ok");

        var result = await _controller.BulkMarkAsPurchased(ids);

        (result as OkObjectResult).Should().NotBeNull();

        _mockMediator.Verify(m => m.Send(
            It.Is<BulkMarkAsPurchasedCommand>(c => c.UserId == _userId && c.ItemIds.SequenceEqual(ids)),
            default), Times.Once);
    }

    [Fact]
    public async Task RemoveItem_ShouldSendCommandWithRouteAndUser_AndReturnOk()
    {
        var itemId = Guid.NewGuid();

        _mockMediator
            .Setup(m => m.Send(It.IsAny<RemoveShoppingListItemCommand>(), default))
            .ReturnsAsync("removed");

        var result = await _controller.RemoveItem(itemId);

        (result as OkObjectResult).Should().NotBeNull();

        _mockMediator.Verify(m => m.Send(
            It.Is<RemoveShoppingListItemCommand>(c => c.ItemId == itemId && c.UserId == _userId),
            default), Times.Once);
    }

    [Fact]
    public async Task GetMyShoppingList_WhenUserClaimIsMissing_ShouldThrowUnauthorizedAccessException()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

        Func<Task> act = async () => await _controller.GetMyShoppingList();

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
