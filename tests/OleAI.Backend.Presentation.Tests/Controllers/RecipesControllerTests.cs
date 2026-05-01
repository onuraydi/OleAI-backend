using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Commands.AddComment;
using OleAI.Backend.Application.Features.Recipes.Commands.ApproveRecipe;
using OleAI.Backend.Application.Features.Recipes.Commands.CreateRecipe;
using OleAI.Backend.Application.Features.Recipes.Commands.DeleteRecipe;
using OleAI.Backend.Application.Features.Recipes.Commands.ToggleInteraction;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Application.Features.Recipes.Queries.GetPublishedRecipes;
using OleAI.Backend.Application.Features.Recipes.Queries.GetRecipeDetail;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Presentation.Controllers;
using Xunit;

namespace OleAI.Backend.Presentation.Tests.Controllers;

public class RecipesControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly RecipesController _controller;

    public RecipesControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new RecipesController();

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
        // Arrange
        var command = new CreateRecipeCommand();
        _mockMediator.Setup(m => m.Send(command, default)).ReturnsAsync("Created");

        // Act
        var result = await _controller.Add(command);

        // Assert
        (result as CreatedResult).Should().NotBeNull();
        _mockMediator.Verify(m => m.Send(command, default), Times.Once);
    }

    [Fact]
    public async Task GetPublishedRecipes_ShouldSendQuery_AndReturnOk()
    {
        // Arrange
        var query = new GetPublishedRecipesQuery();
        var response = new Paginate<RecipeListItemResponse>();
        _mockMediator.Setup(m => m.Send(query, default)).ReturnsAsync(response);

        // Act
        var result = await _controller.GetPublishedRecipes(query);

        // Assert
        (result as OkObjectResult).Should().NotBeNull();
        _mockMediator.Verify(m => m.Send(query, default), Times.Once);
    }

    [Fact]
    public async Task Approve_ShouldSendApproveCommand_AndReturnOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockMediator.Setup(m => m.Send(It.IsAny<ApproveRecipeCommand>(), default)).ReturnsAsync("Approved");

        // Act
        var result = await _controller.Approve(id);

        // Assert
        (result as OkObjectResult).Should().NotBeNull();
        _mockMediator.Verify(m => m.Send(It.Is<ApproveRecipeCommand>(c => c.Id == id), default), Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldSendDeleteCommand_AndReturnOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockMediator.Setup(m => m.Send(It.IsAny<DeleteRecipeCommand>(), default)).ReturnsAsync("Deleted");

        // Act
        var result = await _controller.Delete(id);

        // Assert
        (result as OkObjectResult).Should().NotBeNull();
        _mockMediator.Verify(m => m.Send(It.Is<DeleteRecipeCommand>(c => c.Id == id), default), Times.Once);
    }

    [Fact]
    public async Task ToggleLike_ShouldSendToggleInteractionCommand_AndReturnOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockMediator.Setup(m => m.Send(It.IsAny<ToggleInteractionCommand>(), default)).ReturnsAsync("Liked");

        // Act
        var result = await _controller.ToggleLike(id);

        // Assert
        (result as OkObjectResult).Should().NotBeNull();
        _mockMediator.Verify(m => m.Send(It.Is<ToggleInteractionCommand>(c => c.RecipeId == id && c.InteractionType == "Like"), default), Times.Once);
    }

    [Fact]
    public async Task AddComment_ShouldSendCommandWithRecipeId_AndReturnOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new AddCommentCommand { Content = "Test" };
        _mockMediator.Setup(m => m.Send(command, default)).ReturnsAsync("Commented");

        // Act
        var result = await _controller.AddComment(id, command);

        // Assert
        (result as OkObjectResult).Should().NotBeNull();
        command.RecipeId.Should().Be(id);
        _mockMediator.Verify(m => m.Send(command, default), Times.Once);
    }

    [Fact]
    public async Task GetDetail_ShouldSendGetRecipeDetailQuery_AndReturnOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockMediator.Setup(m => m.Send(It.IsAny<GetRecipeDetailQuery>(), default)).ReturnsAsync(new RecipeDetailResponse());

        // Act
        var result = await _controller.GetDetail(id);

        // Assert
        (result as OkObjectResult).Should().NotBeNull();
        _mockMediator.Verify(m => m.Send(It.Is<GetRecipeDetailQuery>(q => q.Id == id), default), Times.Once);
    }
}
