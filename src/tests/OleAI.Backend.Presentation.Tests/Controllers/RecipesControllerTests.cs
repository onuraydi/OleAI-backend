using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Commands.AddComment;
using OleAI.Backend.Application.Features.Recipes.Commands.ApproveRecipe;
using OleAI.Backend.Application.Features.Recipes.Commands.ChangeRecipeVisibility;
using OleAI.Backend.Application.Features.Recipes.Commands.CreateRecipe;
using OleAI.Backend.Application.Features.Recipes.Commands.DeleteComment;
using OleAI.Backend.Application.Features.Recipes.Commands.DeleteRecipe;
using OleAI.Backend.Application.Features.Recipes.Commands.RejectRecipe;
using OleAI.Backend.Application.Features.Recipes.Commands.StartCooking;
using OleAI.Backend.Application.Features.Recipes.Commands.ToggleInteraction;
using OleAI.Backend.Application.Features.Recipes.Commands.UpdateComment;
using OleAI.Backend.Application.Features.Recipes.Commands.UpdateRecipe;
using OleAI.Backend.Application.Features.Recipes.Dtos.Requests;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Application.Features.Recipes.Queries.GetMyDislikedRecipes;
using OleAI.Backend.Application.Features.Recipes.Queries.GetMyLikedRecipes;
using OleAI.Backend.Application.Features.Recipes.Queries.GetMyRecipes;
using OleAI.Backend.Application.Features.Recipes.Queries.GetMySavedRecipes;
using OleAI.Backend.Application.Features.Recipes.Queries.GetPendingAdminRecipes;
using OleAI.Backend.Application.Features.Recipes.Queries.GetPublishedRecipes;
using OleAI.Backend.Application.Features.Recipes.Queries.GetRecipeDetail;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Enums;
using OleAI.Backend.Presentation.Controllers;
using Xunit;

namespace OleAI.Backend.Presentation.Tests.Controllers;

public class RecipesControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly RecipesController _controller;

    public RecipesControllerTests()
    {
        _controller = new RecipesController();

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(x => x.GetService(typeof(IMediator))).Returns(_mediator.Object);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                RequestServices = serviceProvider.Object
            }
        };
    }

    [Fact]
    public async Task Add_ShouldSendCommand_AndReturnCreatedResult()
    {
        var command = new CreateRecipeCommand();
        _mediator.Setup(x => x.Send(command, default)).ReturnsAsync("created");

        var result = await _controller.Add(command);

        result.Should().BeOfType<CreatedResult>();
        _mediator.Verify(x => x.Send(command, default), Times.Once);
    }

    [Fact]
    public async Task GetPendingApprovals_ShouldSendQuery_AndReturnOkResult()
    {
        var query = new GetPendingAdminRecipesQuery();
        var response = new Paginate<PendingAdminRecipeListResponse>();
        _mediator.Setup(x => x.Send(query, default)).ReturnsAsync(response);

        var result = await _controller.GetPendingApprovals(query);

        result.Should().BeOfType<OkObjectResult>();
        _mediator.Verify(x => x.Send(query, default), Times.Once);
    }

    [Fact]
    public async Task Approve_ShouldSendCommandWithRouteId()
    {
        var id = Guid.NewGuid();
        _mediator.Setup(x => x.Send(It.IsAny<ApproveRecipeCommand>(), default)).ReturnsAsync("approved");

        var result = await _controller.Approve(id);

        result.Should().BeOfType<OkObjectResult>();
        _mediator.Verify(x => x.Send(It.Is<ApproveRecipeCommand>(c => c.Id == id), default), Times.Once);
    }

    [Fact]
    public async Task Reject_ShouldSetRouteIdOnCommand_AndReturnOk()
    {
        var id = Guid.NewGuid();
        var command = new RejectRecipeCommand { Reason = "spam" };
        _mediator.Setup(x => x.Send(It.IsAny<RejectRecipeCommand>(), default)).ReturnsAsync("rejected");

        var result = await _controller.Reject(id, command);

        result.Should().BeOfType<OkObjectResult>();
        command.Id.Should().Be(id);
        _mediator.Verify(x => x.Send(command, default), Times.Once);
    }

    [Fact]
    public async Task Update_ShouldOverrideRouteIdOnCommand_AndReturnOk()
    {
        var id = Guid.NewGuid();
        var command = new UpdateRecipeCommand { Request = new UpdateRecipeRequest { Id = Guid.NewGuid(), Title = "Yeni", Description = "Açıklama", Visibility = RecipeVisibility.Private } };
        _mediator.Setup(x => x.Send(It.IsAny<UpdateRecipeCommand>(), default)).ReturnsAsync("updated");

        var result = await _controller.Update(id, command);

        result.Should().BeOfType<OkObjectResult>();
        command.Request.Id.Should().Be(id);
        _mediator.Verify(x => x.Send(command, default), Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldSendCommandWithRouteId()
    {
        var id = Guid.NewGuid();
        _mediator.Setup(x => x.Send(It.IsAny<DeleteRecipeCommand>(), default)).ReturnsAsync("deleted");

        var result = await _controller.Delete(id);

        result.Should().BeOfType<OkObjectResult>();
        _mediator.Verify(x => x.Send(It.Is<DeleteRecipeCommand>(c => c.Id == id), default), Times.Once);
    }

    [Fact]
    public async Task ChangeVisibility_ShouldSetRouteIdOnCommand_AndReturnOk()
    {
        var id = Guid.NewGuid();
        var command = new ChangeRecipeVisibilityCommand { Visibility = RecipeVisibility.Public };
        _mediator.Setup(x => x.Send(It.IsAny<ChangeRecipeVisibilityCommand>(), default)).ReturnsAsync("changed");

        var result = await _controller.ChangeVisibility(id, command);

        result.Should().BeOfType<OkObjectResult>();
        command.Id.Should().Be(id);
        _mediator.Verify(x => x.Send(command, default), Times.Once);
    }

    [Fact]
    public async Task GetPublishedRecipes_ShouldSendQuery_AndReturnOkResult()
    {
        var query = new GetPublishedRecipesQuery();
        var response = new Paginate<RecipeListItemResponse>();
        _mediator.Setup(x => x.Send(query, default)).ReturnsAsync(response);

        var result = await _controller.GetPublishedRecipes(query);

        result.Should().BeOfType<OkObjectResult>();
        _mediator.Verify(x => x.Send(query, default), Times.Once);
    }

    [Fact]
    public async Task GetMyRecipes_ShouldSendQuery_AndReturnOkResult()
    {
        var query = new GetMyRecipesQuery();
        var response = new Paginate<RecipeListItemResponse>();
        _mediator.Setup(x => x.Send(query, default)).ReturnsAsync(response);

        var result = await _controller.GetMyRecipes(query);

        result.Should().BeOfType<OkObjectResult>();
        _mediator.Verify(x => x.Send(query, default), Times.Once);
    }

    [Fact]
    public async Task GetDetail_ShouldSendQueryWithRouteId_AndReturnOkResult()
    {
        var id = Guid.NewGuid();
        _mediator.Setup(x => x.Send(It.IsAny<GetRecipeDetailQuery>(), default)).ReturnsAsync(new RecipeDetailResponse());

        var result = await _controller.GetDetail(id);

        result.Should().BeOfType<OkObjectResult>();
        _mediator.Verify(x => x.Send(It.Is<GetRecipeDetailQuery>(q => q.Id == id), default), Times.Once);
    }

    [Fact]
    public async Task StartCooking_ShouldSendCommandWithRecipeId_AndReturnOkResult()
    {
        var id = Guid.NewGuid();
        _mediator.Setup(x => x.Send(It.IsAny<StartCookingCommand>(), default)).ReturnsAsync("started");

        var result = await _controller.StartCooking(id);

        result.Should().BeOfType<OkObjectResult>();
        _mediator.Verify(x => x.Send(It.Is<StartCookingCommand>(c => c.RecipeId == id), default), Times.Once);
    }

    [Fact]
    public async Task ToggleLike_ShouldSendLikeCommand_AndReturnOkResult()
    {
        var id = Guid.NewGuid();
        _mediator.Setup(x => x.Send(It.IsAny<ToggleInteractionCommand>(), default)).ReturnsAsync("liked");

        var result = await _controller.ToggleLike(id);

        result.Should().BeOfType<OkObjectResult>();
        _mediator.Verify(x => x.Send(It.Is<ToggleInteractionCommand>(c => c.RecipeId == id && c.InteractionType == "Like"), default), Times.Once);
    }

    [Fact]
    public async Task ToggleDislike_ShouldSendDislikeCommand_AndReturnOkResult()
    {
        var id = Guid.NewGuid();
        _mediator.Setup(x => x.Send(It.IsAny<ToggleInteractionCommand>(), default)).ReturnsAsync("disliked");

        var result = await _controller.ToggleDislike(id);

        result.Should().BeOfType<OkObjectResult>();
        _mediator.Verify(x => x.Send(It.Is<ToggleInteractionCommand>(c => c.RecipeId == id && c.InteractionType == "Dislike"), default), Times.Once);
    }

    [Fact]
    public async Task ToggleSave_ShouldSendSaveCommand_AndReturnOkResult()
    {
        var id = Guid.NewGuid();
        _mediator.Setup(x => x.Send(It.IsAny<ToggleInteractionCommand>(), default)).ReturnsAsync("saved");

        var result = await _controller.ToggleSave(id);

        result.Should().BeOfType<OkObjectResult>();
        _mediator.Verify(x => x.Send(It.Is<ToggleInteractionCommand>(c => c.RecipeId == id && c.InteractionType == "Save"), default), Times.Once);
    }

    [Fact]
    public async Task AddComment_ShouldSetRecipeIdOnCommand_AndReturnOkResult()
    {
        var id = Guid.NewGuid();
        var command = new AddCommentCommand { Content = "harika" };
        _mediator.Setup(x => x.Send(It.IsAny<AddCommentCommand>(), default)).ReturnsAsync("comment-added");

        var result = await _controller.AddComment(id, command);

        result.Should().BeOfType<OkObjectResult>();
        command.RecipeId.Should().Be(id);
        _mediator.Verify(x => x.Send(command, default), Times.Once);
    }

    [Fact]
    public async Task UpdateComment_ShouldSetCommentIdOnCommand_AndReturnOkResult()
    {
        var id = Guid.NewGuid();
        var command = new UpdateCommentCommand { NewContent = "güncel" };
        _mediator.Setup(x => x.Send(It.IsAny<UpdateCommentCommand>(), default)).ReturnsAsync("updated");

        var result = await _controller.UpdateComment(id, command);

        result.Should().BeOfType<OkObjectResult>();
        command.CommentId.Should().Be(id);
        _mediator.Verify(x => x.Send(command, default), Times.Once);
    }

    [Fact]
    public async Task DeleteComment_ShouldSendDeleteCommandWithCommentId()
    {
        var id = Guid.NewGuid();
        _mediator.Setup(x => x.Send(It.IsAny<DeleteCommentCommand>(), default)).ReturnsAsync("deleted");

        var result = await _controller.DeleteComment(id);

        result.Should().BeOfType<OkObjectResult>();
        _mediator.Verify(x => x.Send(It.Is<DeleteCommentCommand>(c => c.CommentId == id), default), Times.Once);
    }

    [Fact]
    public async Task GetMySavedRecipes_ShouldSendQuery_AndReturnOkResult()
    {
        var query = new GetMySavedRecipesQuery();
        _mediator.Setup(x => x.Send(query, default)).ReturnsAsync(new Paginate<RecipeListItemResponse>());

        var result = await _controller.GetMySavedRecipes(query);

        result.Should().BeOfType<OkObjectResult>();
        _mediator.Verify(x => x.Send(query, default), Times.Once);
    }

    [Fact]
    public async Task GetMyLikedRecipes_ShouldSendQuery_AndReturnOkResult()
    {
        var query = new GetMyLikedRecipesQuery();
        _mediator.Setup(x => x.Send(query, default)).ReturnsAsync(new Paginate<RecipeListItemResponse>());

        var result = await _controller.GetMyLikedRecipes(query);

        result.Should().BeOfType<OkObjectResult>();
        _mediator.Verify(x => x.Send(query, default), Times.Once);
    }

    [Fact]
    public async Task GetMyDisikedRecipes_ShouldSendQuery_AndReturnOkResult()
    {
        var query = new GetMyDislikedRecipesQuery();
        _mediator.Setup(x => x.Send(query, default)).ReturnsAsync(new Paginate<RecipeListItemResponse>());

        var result = await _controller.GetMyDisikedRecipes(query);

        result.Should().BeOfType<OkObjectResult>();
        _mediator.Verify(x => x.Send(query, default), Times.Once);
    }
}
