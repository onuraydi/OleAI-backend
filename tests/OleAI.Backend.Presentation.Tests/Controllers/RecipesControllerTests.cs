using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Commands.AddComment;
using OleAI.Backend.Application.Features.Recipes.Commands.AnalyzeRecipeWithLlm;
using OleAI.Backend.Application.Features.Recipes.Commands.ApproveRecipe;
using OleAI.Backend.Application.Features.Recipes.Commands.ChangeRecipeVisibility;
using OleAI.Backend.Application.Features.Recipes.Commands.CreateRecipe;
using OleAI.Backend.Application.Features.Recipes.Commands.DeleteComment;
using OleAI.Backend.Application.Features.Recipes.Commands.DeleteRecipe;
using OleAI.Backend.Application.Features.Recipes.Commands.RejectRecipe;
using OleAI.Backend.Application.Features.Recipes.Commands.StartCooking;
using OleAI.Backend.Application.Features.Recipes.Commands.SyncAllToAi;
using OleAI.Backend.Application.Features.Recipes.Commands.ToggleInteraction;
using OleAI.Backend.Application.Features.Recipes.Commands.UpdateComment;
using OleAI.Backend.Application.Features.Recipes.Commands.UpdateRecipe;
using OleAI.Backend.Application.Features.Recipes.Dtos.Requests;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Application.Features.Recipes.Queries.GetAllAdminRecipes;
using OleAI.Backend.Application.Features.Recipes.Queries.GetMyComments;
using OleAI.Backend.Application.Features.Recipes.Queries.GetMyDislikedRecipes;
using OleAI.Backend.Application.Features.Recipes.Queries.GetMyLikedRecipes;
using OleAI.Backend.Application.Features.Recipes.Queries.GetMyRecipes;
using OleAI.Backend.Application.Features.Recipes.Queries.GetMySavedRecipes;
using OleAI.Backend.Application.Features.Recipes.Queries.GetPendingAdminRecipes;
using OleAI.Backend.Application.Features.Recipes.Queries.GetPublishedRecipes;
using OleAI.Backend.Application.Features.Recipes.Queries.GetRecipeDetail;
using OleAI.Backend.Application.Features.Recipes.Queries.GetRecipesForAi;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Presentation.Controllers;
using Xunit;

namespace OleAI.Backend.Presentation.Tests.Controllers
{
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

        // POST endpoints
        [Fact] public async Task Add_ShouldReturnCreated() { var cmd = new CreateRecipeCommand(); _mockMediator.Setup(m => m.Send(cmd, default)).ReturnsAsync("created"); var result = await _controller.Add(cmd); (result as CreatedResult).Should().NotBeNull(); }
        [Fact] public async Task Approve_ShouldReturnOk() { var id = Guid.NewGuid(); _mockMediator.Setup(m => m.Send(It.IsAny<ApproveRecipeCommand>(), default)).ReturnsAsync("ok"); var result = await _controller.Approve(id, null); (result as OkObjectResult).Should().NotBeNull(); }
        [Fact] public async Task Reject_ShouldReturnOk() { var id = Guid.NewGuid(); _mockMediator.Setup(m => m.Send(It.IsAny<RejectRecipeCommand>(), default)).ReturnsAsync("ok"); var result = await _controller.Reject(id, new RejectRecipeCommand()); (result as OkObjectResult).Should().NotBeNull(); }
        [Fact] public async Task SyncAllToAi_ShouldReturnOk() { _mockMediator.Setup(m => m.Send(It.IsAny<SyncAllToAiCommand>(), default)).ReturnsAsync("ok"); var result = await _controller.SyncAllToAi(); (result as OkObjectResult).Should().NotBeNull(); }
        [Fact] public async Task AnalyzeWithAi_ShouldReturnOk() { _mockMediator.Setup(m => m.Send(It.IsAny<AnalyzeRecipeWithLlmCommand>(), default)).ReturnsAsync("ok"); var result = await _controller.AnalyzeWithAi(Guid.NewGuid()); (result as OkObjectResult).Should().NotBeNull(); }
        [Fact] public async Task Restore_ShouldReturnOk() { _mockMediator.Setup(m => m.Send(It.IsAny<OleAI.Backend.Application.Features.Recipes.Commands.RestoreRecipe.RestoreRecipeCommand>(), default)).ReturnsAsync("ok"); var result = await _controller.Restore(Guid.NewGuid()); (result as OkObjectResult).Should().NotBeNull(); }
        [Fact] public async Task StartCooking_ShouldReturnOk() { _mockMediator.Setup(m => m.Send(It.IsAny<StartCookingCommand>(), default)).ReturnsAsync(new List<OleAI.Backend.Application.Features.Ingredients.Dtos.Responses.IngredientResponse>()); var result = await _controller.StartCooking(Guid.NewGuid(), new StartCookingCommand()); (result as OkObjectResult).Should().NotBeNull(); }
        [Fact] public async Task ToggleLike_ShouldReturnOk() { _mockMediator.Setup(m => m.Send(It.IsAny<ToggleInteractionCommand>(), default)).ReturnsAsync("ok"); var result = await _controller.ToggleLike(Guid.NewGuid()); (result as OkObjectResult).Should().NotBeNull(); }
        [Fact] public async Task ToggleDislike_ShouldReturnOk() { _mockMediator.Setup(m => m.Send(It.IsAny<ToggleInteractionCommand>(), default)).ReturnsAsync("ok"); var result = await _controller.ToggleDislike(Guid.NewGuid()); (result as OkObjectResult).Should().NotBeNull(); }
        [Fact] public async Task ToggleSave_ShouldReturnOk() { _mockMediator.Setup(m => m.Send(It.IsAny<ToggleInteractionCommand>(), default)).ReturnsAsync("ok"); var result = await _controller.ToggleSave(Guid.NewGuid()); (result as OkObjectResult).Should().NotBeNull(); }
        [Fact] public async Task AddComment_ShouldReturnOk() { _mockMediator.Setup(m => m.Send(It.IsAny<AddCommentCommand>(), default)).ReturnsAsync("ok"); var result = await _controller.AddComment(Guid.NewGuid(), new AddCommentCommand()); (result as OkObjectResult).Should().NotBeNull(); }

        // GET endpoints
        [Fact] public async Task GetPendingApprovals_ShouldReturnOk() { _mockMediator.Setup(m => m.Send(It.IsAny<GetPendingAdminRecipesQuery>(), default)).ReturnsAsync(new Paginate<PendingAdminRecipeListResponse>()); var result = await _controller.GetPendingApprovals(new GetPendingAdminRecipesQuery()); (result as OkObjectResult).Should().NotBeNull(); }
        [Fact] public async Task GetAllAdminRecipes_ShouldReturnOk() { _mockMediator.Setup(m => m.Send(It.IsAny<GetAllAdminRecipesQuery>(), default)).ReturnsAsync(new Paginate<AdminRecipeListItemDto>()); var result = await _controller.GetAllAdminRecipes(new GetAllAdminRecipesQuery()); (result as OkObjectResult).Should().NotBeNull(); }
        [Fact] public async Task GetDeletedAdminRecipes_ShouldReturnOk() { _mockMediator.Setup(m => m.Send(It.IsAny<OleAI.Backend.Application.Features.Recipes.Queries.GetDeletedAdminRecipes.GetDeletedAdminRecipesQuery>(), default)).ReturnsAsync(new Paginate<AdminRecipeListItemDto>()); var result = await _controller.GetDeletedAdminRecipes(new OleAI.Backend.Application.Features.Recipes.Queries.GetDeletedAdminRecipes.GetDeletedAdminRecipesQuery()); (result as OkObjectResult).Should().NotBeNull(); }
        [Fact] public async Task GetPublishedRecipes_ShouldReturnOk() { _mockMediator.Setup(m => m.Send(It.IsAny<GetPublishedRecipesQuery>(), default)).ReturnsAsync(new Paginate<RecipeListItemResponse>()); var result = await _controller.GetPublishedRecipes(new GetPublishedRecipesQuery()); (result as OkObjectResult).Should().NotBeNull(); }
        [Fact] public async Task GetMyRecipes_ShouldReturnOk() { _mockMediator.Setup(m => m.Send(It.IsAny<GetMyRecipesQuery>(), default)).ReturnsAsync(new Paginate<RecipeListItemResponse>()); var result = await _controller.GetMyRecipes(new GetMyRecipesQuery()); (result as OkObjectResult).Should().NotBeNull(); }
        [Fact] public async Task GetMyComments_ShouldReturnOk() { _mockMediator.Setup(m => m.Send(It.IsAny<GetMyCommentsQuery>(), default)).ReturnsAsync(new Paginate<MyCommentResponse>()); var result = await _controller.GetMyComments(new GetMyCommentsQuery()); (result as OkObjectResult).Should().NotBeNull(); }
        [Fact] public async Task GetDetail_ShouldReturnOk() { _mockMediator.Setup(m => m.Send(It.IsAny<GetRecipeDetailQuery>(), default)).ReturnsAsync(new RecipeDetailResponse()); var result = await _controller.GetDetail(Guid.NewGuid()); (result as OkObjectResult).Should().NotBeNull(); }
        [Fact] public async Task GetForAi_ShouldReturnOk() { _mockMediator.Setup(m => m.Send(It.IsAny<GetRecipesForAiQuery>(), default)).ReturnsAsync(new List<OleAI.Backend.Application.Features.Recipes.Queries.GetRecipesForAi.RecipeForAiDto>()); var result = await _controller.GetForAi(); (result as OkObjectResult).Should().NotBeNull(); }
        [Fact] public async Task GetMissingIngredients_ShouldReturnOk() { _mockMediator.Setup(m => m.Send(It.IsAny<OleAI.Backend.Application.Features.Recipes.Queries.GetMissingIngredients.GetMissingIngredientsQuery>(), default)).ReturnsAsync(new List<OleAI.Backend.Application.Features.Ingredients.Dtos.Responses.IngredientResponse>()); var result = await _controller.GetMissingIngredients(Guid.NewGuid()); (result as OkObjectResult).Should().NotBeNull(); }
        [Fact] public async Task GetComments_ShouldReturnOk() { _mockMediator.Setup(m => m.Send(It.IsAny<OleAI.Backend.Application.Features.Recipes.Queries.GetRecipeComments.GetRecipeCommentsQuery>(), default)).ReturnsAsync(new Paginate<OleAI.Backend.Application.Features.Recipes.Dtos.Responses.RecipeCommentResponse>()); var result = await _controller.GetComments(Guid.NewGuid(), 0, 10); (result as OkObjectResult).Should().NotBeNull(); }
        [Fact] public async Task GetMySavedRecipes_ShouldReturnOk() { _mockMediator.Setup(m => m.Send(It.IsAny<GetMySavedRecipesQuery>(), default)).ReturnsAsync(new Paginate<RecipeListItemResponse>()); var result = await _controller.GetMySavedRecipes(new GetMySavedRecipesQuery()); (result as OkObjectResult).Should().NotBeNull(); }
        [Fact] public async Task GetMyLikedRecipes_ShouldReturnOk() { _mockMediator.Setup(m => m.Send(It.IsAny<GetMyLikedRecipesQuery>(), default)).ReturnsAsync(new Paginate<RecipeListItemResponse>()); var result = await _controller.GetMyLikedRecipes(new GetMyLikedRecipesQuery()); (result as OkObjectResult).Should().NotBeNull(); }
        [Fact] public async Task GetMyDisikedRecipes_ShouldReturnOk() { _mockMediator.Setup(m => m.Send(It.IsAny<GetMyDislikedRecipesQuery>(), default)).ReturnsAsync(new Paginate<RecipeListItemResponse>()); var result = await _controller.GetMyDisikedRecipes(new GetMyDislikedRecipesQuery()); (result as OkObjectResult).Should().NotBeNull(); }

        // PUT & PATCH endpoints
        [Fact] public async Task Update_ShouldReturnOk() { var cmd = new UpdateRecipeCommand { Request = new UpdateRecipeRequest() }; _mockMediator.Setup(m => m.Send(It.IsAny<UpdateRecipeCommand>(), default)).ReturnsAsync("ok"); var result = await _controller.Update(Guid.NewGuid(), cmd); (result as OkObjectResult).Should().NotBeNull(); }
        [Fact] public async Task ChangeVisibility_ShouldReturnOk() { var cmd = new ChangeRecipeVisibilityCommand(); _mockMediator.Setup(m => m.Send(It.IsAny<ChangeRecipeVisibilityCommand>(), default)).ReturnsAsync("ok"); var result = await _controller.ChangeVisibility(Guid.NewGuid(), cmd); (result as OkObjectResult).Should().NotBeNull(); }
        [Fact] public async Task UpdateComment_ShouldReturnOk() { var cmd = new UpdateCommentCommand(); _mockMediator.Setup(m => m.Send(It.IsAny<UpdateCommentCommand>(), default)).ReturnsAsync("ok"); var result = await _controller.UpdateComment(Guid.NewGuid(), cmd); (result as OkObjectResult).Should().NotBeNull(); }

        // DELETE endpoints
        [Fact] public async Task Delete_ShouldReturnOk() { _mockMediator.Setup(m => m.Send(It.IsAny<DeleteRecipeCommand>(), default)).ReturnsAsync("ok"); var result = await _controller.Delete(Guid.NewGuid()); (result as OkObjectResult).Should().NotBeNull(); }
        [Fact] public async Task HardDelete_ShouldReturnOk() { _mockMediator.Setup(m => m.Send(It.IsAny<OleAI.Backend.Application.Features.Recipes.Commands.HardDeleteRecipe.HardDeleteRecipeCommand>(), default)).ReturnsAsync("ok"); var result = await _controller.HardDelete(Guid.NewGuid()); (result as OkObjectResult).Should().NotBeNull(); }
        [Fact] public async Task DeleteComment_ShouldReturnOk() { _mockMediator.Setup(m => m.Send(It.IsAny<DeleteCommentCommand>(), default)).ReturnsAsync("ok"); var result = await _controller.DeleteComment(Guid.NewGuid()); (result as OkObjectResult).Should().NotBeNull(); }
    }
}
