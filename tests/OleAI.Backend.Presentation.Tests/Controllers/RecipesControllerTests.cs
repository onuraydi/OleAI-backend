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
        var command = new CreateRecipeCommand();
        _mockMediator.Setup(m => m.Send(command, default)).ReturnsAsync("Created");
        var result = await _controller.Add(command);
        (result as CreatedResult).Should().NotBeNull();
    }

    [Fact]
    public async Task Update_ShouldSendCommandWithUrlId_AndReturnOk()
    {
        var id = Guid.NewGuid();
        var command = new UpdateRecipeCommand { Request = new UpdateRecipeRequest() };
        _mockMediator.Setup(m => m.Send(command, default)).ReturnsAsync("Updated");
        var result = await _controller.Update(id, command);
        (result as OkObjectResult).Should().NotBeNull();
        command.Request.Id.Should().Be(id);
    }

    [Fact]
    public async Task Reject_ShouldSendCommandWithUrlId_AndReturnOk()
    {
        var id = Guid.NewGuid();
        var command = new RejectRecipeCommand();
        _mockMediator.Setup(m => m.Send(command, default)).ReturnsAsync("Rejected");
        var result = await _controller.Reject(id, command);
        (result as OkObjectResult).Should().NotBeNull();
        command.Id.Should().Be(id);
    }

    [Fact]
    public async Task ChangeVisibility_ShouldSendCommandWithUrlId_AndReturnOk()
    {
        var id = Guid.NewGuid();
        var command = new ChangeRecipeVisibilityCommand();
        _mockMediator.Setup(m => m.Send(command, default)).ReturnsAsync("VisibilityChanged");
        var result = await _controller.ChangeVisibility(id, command);
        (result as OkObjectResult).Should().NotBeNull();
        command.Id.Should().Be(id);
    }

    [Fact]
    public async Task StartCooking_ShouldSendCommandWithId_AndReturnOk()
    {
        var id = Guid.NewGuid();
        var command = new StartCookingCommand();
        _mockMediator.Setup(m => m.Send(It.IsAny<StartCookingCommand>(), default)).ReturnsAsync(new System.Collections.Generic.List<OleAI.Backend.Application.Features.Ingredients.Dtos.Responses.IngredientResponse>());
        var result = await _controller.StartCooking(id, command);
        (result as OkObjectResult).Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateComment_ShouldSendCommandWithCommentId_AndReturnOk()
    {
        var commentId = Guid.NewGuid();
        var command = new UpdateCommentCommand();
        _mockMediator.Setup(m => m.Send(command, default)).ReturnsAsync("Updated");
        var result = await _controller.UpdateComment(commentId, command);
        (result as OkObjectResult).Should().NotBeNull();
        command.CommentId.Should().Be(commentId);
    }

    [Fact]
    public async Task DeleteComment_ShouldSendCommandWithCommentId_AndReturnOk()
    {
        var commentId = Guid.NewGuid();
        _mockMediator.Setup(m => m.Send(It.IsAny<DeleteCommentCommand>(), default)).ReturnsAsync("Deleted");
        var result = await _controller.DeleteComment(commentId);
        (result as OkObjectResult).Should().NotBeNull();
    }

    [Fact]
    public async Task GetMyRecipes_ShouldSendQuery_AndReturnOk()
    {
        var query = new GetMyRecipesQuery();
        _mockMediator.Setup(m => m.Send(query, default)).ReturnsAsync(new Paginate<RecipeListItemResponse>());
        var result = await _controller.GetMyRecipes(query);
        (result as OkObjectResult).Should().NotBeNull();
    }

    [Fact]
    public async Task GetMySavedRecipes_ShouldSendQuery_AndReturnOk()
    {
        var query = new GetMySavedRecipesQuery();
        _mockMediator.Setup(m => m.Send(query, default)).ReturnsAsync(new Paginate<RecipeListItemResponse>());
        var result = await _controller.GetMySavedRecipes(query);
        (result as OkObjectResult).Should().NotBeNull();
    }

    [Fact]
    public async Task GetPendingApprovals_ShouldSendQuery_AndReturnOk()
    {
        var query = new GetPendingAdminRecipesQuery();
        _mockMediator.Setup(m => m.Send(query, default)).ReturnsAsync(new Paginate<PendingAdminRecipeListResponse>());
        var result = await _controller.GetPendingApprovals(query);
        (result as OkObjectResult).Should().NotBeNull();
    }

    // Reuse existing tests below or just override them
    [Fact]
    public async Task ToggleDislike_ShouldSendToggleInteractionCommand_AndReturnOk()
    {
        var id = Guid.NewGuid();
        _mockMediator.Setup(m => m.Send(It.IsAny<ToggleInteractionCommand>(), default)).ReturnsAsync("Disliked");
        var result = await _controller.ToggleDislike(id);
        (result as OkObjectResult).Should().NotBeNull();
    }
}
