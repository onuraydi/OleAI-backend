using System.Linq.Expressions;
using System.Security.Claims;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
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
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Features.Recipes.Dtos.Requests;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.ImageStorage;
using OleAI.Backend.Application.Services.Llm;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Application.Services.UserProfiles;
using OleAI.Backend.Application.Tests.TestInfrastructure;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Commands;

public class CreateRecipeCommandHandlerTests
{
    private readonly Mock<IRecipeRepository> _recipeRepository = new();
    private readonly Mock<IImageService> _imageService = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();
    private readonly Mock<ILocalLlmService> _localLlmService = new();
    private readonly Mock<IIngredientRepository> _ingredientRepository = new();
    private readonly Mock<IRecipeCommentRepository> _commentRepository = new();
    private readonly RecipeBusinessRules _rules;
    private readonly CreateRecipeCommandHandler _handler;

    public CreateRecipeCommandHandlerTests()
    {
        _rules = new RecipeBusinessRules(_ingredientRepository.Object, _commentRepository.Object);
        _handler = new CreateRecipeCommandHandler(_recipeRepository.Object, _imageService.Object, _httpContextAccessor.Object, _rules, _localLlmService.Object);
    }

    [Fact]
    public async Task Handle_WhenUserClaimMissing_ShouldThrowUnauthorizedAccessException()
    {
        var command = BuildCommand(RecipeVisibility.Private);
        SetupIngredientCount(command);
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage(RecipeMessages.UnauthorizedAccess);
    }

    [Fact]
    public async Task Handle_WhenVisibilityIsPrivate_ShouldCreateRecipeAndReturnPrivateMessage()
    {
        var userId = Guid.NewGuid();
        var command = BuildCommand(RecipeVisibility.Private);
        SetupIngredientCount(command);
        SetupHttpContext(userId);

        _imageService
            .Setup(x => x.UploadAsync(It.IsAny<IFormFile>(), "recipes/main", It.IsAny<CancellationToken>()))
            .ReturnsAsync(("https://cdn.example.com/main.jpg", "main-public-id"));

        Recipe? capturedRecipe = null;
        _recipeRepository
            .Setup(x => x.AddAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()))
            .Callback<Recipe, CancellationToken>((recipe, _) => capturedRecipe = recipe)
            .ReturnsAsync((Recipe recipe, CancellationToken _) => recipe);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(RecipeMessages.RecipeCreatedAndSavedAsPrivate);
        capturedRecipe.Should().NotBeNull();
        capturedRecipe!.UserId.Should().Be(userId);
        capturedRecipe.Visibility.Should().Be(RecipeVisibility.Private);
        capturedRecipe.Status.Should().Be(RecipeStatus.Published);
        capturedRecipe.RecipeImages.Should().ContainSingle(x => x.IsMain && x.ImageUrl == "https://cdn.example.com/main.jpg");
        _imageService.Verify(x => x.UploadAsync(It.IsAny<IFormFile>(), "recipes/main", It.IsAny<CancellationToken>()), Times.Once);
        _imageService.Verify(x => x.UploadAsync(It.IsAny<IFormFile>(), "recipes/steps", It.IsAny<CancellationToken>()), Times.Never);
        _recipeRepository.Verify(x => x.AddAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenVisibilityIsPublicAndLlmAcceptsContent_ShouldQueueForAdminApproval()
    {
        var userId = Guid.NewGuid();
        var command = BuildCommand(RecipeVisibility.Public, includeStepImage: true);
        SetupIngredientCount(command);
        SetupHttpContext(userId);

        _imageService
            .Setup(x => x.UploadAsync(It.IsAny<IFormFile>(), "recipes/main", It.IsAny<CancellationToken>()))
            .ReturnsAsync(("https://cdn.example.com/main.jpg", "main-public-id"));
        _imageService
            .Setup(x => x.UploadAsync(It.IsAny<IFormFile>(), "recipes/steps", It.IsAny<CancellationToken>()))
            .ReturnsAsync(("https://cdn.example.com/step-1.jpg", "step-public-id"));

        _localLlmService
            .Setup(x => x.CheckRecipeContentForSpamAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        Recipe? capturedRecipe = null;
        _recipeRepository
            .Setup(x => x.AddAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()))
            .Callback<Recipe, CancellationToken>((recipe, _) => capturedRecipe = recipe)
            .ReturnsAsync((Recipe recipe, CancellationToken _) => recipe);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(RecipeMessages.RecipeCreatedAndPendingAdminApproval);
        capturedRecipe.Should().NotBeNull();
        capturedRecipe!.Visibility.Should().Be(RecipeVisibility.Public);
        capturedRecipe.Status.Should().Be(RecipeStatus.PendingAdminApproval);
        capturedRecipe.RecipeImages.Should().HaveCount(2);
        _localLlmService.Verify(x => x.CheckRecipeContentForSpamAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _imageService.Verify(x => x.UploadAsync(It.IsAny<IFormFile>(), "recipes/main", It.IsAny<CancellationToken>()), Times.Once);
        _imageService.Verify(x => x.UploadAsync(It.IsAny<IFormFile>(), "recipes/steps", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenVisibilityIsPublicAndLlmRejectsContent_ShouldReturnRejectedMessage()
    {
        var userId = Guid.NewGuid();
        var command = BuildCommand(RecipeVisibility.Public);
        SetupIngredientCount(command);
        SetupHttpContext(userId);

        _imageService
            .Setup(x => x.UploadAsync(It.IsAny<IFormFile>(), "recipes/main", It.IsAny<CancellationToken>()))
            .ReturnsAsync(("https://cdn.example.com/main.jpg", "main-public-id"));

        _localLlmService
            .Setup(x => x.CheckRecipeContentForSpamAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Recipe? capturedRecipe = null;
        _recipeRepository
            .Setup(x => x.AddAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()))
            .Callback<Recipe, CancellationToken>((recipe, _) => capturedRecipe = recipe)
            .ReturnsAsync((Recipe recipe, CancellationToken _) => recipe);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(RecipeMessages.RecipeRejectedDueToSystemStandards);
        capturedRecipe.Should().NotBeNull();
        capturedRecipe!.Status.Should().Be(RecipeStatus.Rejected);
        _recipeRepository.Verify(x => x.AddAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private void SetupHttpContext(Guid userId)
    {
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(RecipeTestHelpers.CreateHttpContext(userId));
    }

    private void SetupIngredientCount(CreateRecipeCommand command)
    {
        _ingredientRepository
            .Setup(x => x.GetCountAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(command.Ingredients.Select(i => i.IngredientId).Distinct().Count());
    }

    private static CreateRecipeCommand BuildCommand(RecipeVisibility visibility, bool includeStepImage = false)
    {
        return new CreateRecipeCommand
        {
            CreateRecipeRequest = new CreateRecipeRequest
            {
                Title = "Mercimek Çorbası",
                Description = "Klasik tarif",
                Visibility = visibility,
                MainImage = RecipeTestHelpers.CreateFormFile("main-image", "main.jpg")
            },
            Ingredients =
            [
                new CreateRecipeIngredientRequest { IngredientId = Guid.NewGuid(), Amount = 1.5m, UnitId = 1 },
                new CreateRecipeIngredientRequest { IngredientId = Guid.NewGuid(), Amount = 2, UnitId = 2 }
            ],
            Steps =
            [
                new CreateRecipeStepRequest
                {
                    StepNumber = 1,
                    Instruction = "Karıştır",
                    StepImage = includeStepImage ? RecipeTestHelpers.CreateFormFile("step-image", "step.jpg") : null
                }
            ]
        };
    }
}

public class UpdateRecipeCommandHandlerTests
{
    private readonly Mock<IRecipeRepository> _recipeRepository = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();
    private readonly Mock<ILocalLlmService> _localLlmService = new();
    private readonly Mock<IImageService> _imageService = new();
    private readonly Mock<IIngredientRepository> _ingredientRepository = new();
    private readonly Mock<IRecipeCommentRepository> _commentRepository = new();
    private readonly RecipeBusinessRules _rules;
    private readonly UpdateRecipeCommandHandler _handler;

    public UpdateRecipeCommandHandlerTests()
    {
        _rules = new RecipeBusinessRules(_ingredientRepository.Object, _commentRepository.Object);
        _handler = new UpdateRecipeCommandHandler(_recipeRepository.Object, _rules, _httpContextAccessor.Object, _localLlmService.Object, _imageService.Object);
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_ShouldUpdateRecipeAndReplaceImages()
    {
        var userId = Guid.NewGuid();
        var ownerId = userId;
        var ingredientId = Guid.NewGuid();
        var recipe = BuildExistingRecipe(ownerId, ingredientId);
        var command = BuildUpdateCommand(recipe.Id, ingredientId);

        SetupHttpContext(userId);
        SetupIngredientCount(command);

        _recipeRepository
            .Setup(x => x.GetAsync(
                It.IsAny<Expression<Func<Recipe, bool>>>(),
                It.IsAny<Func<IQueryable<Recipe>, IIncludableQueryable<Recipe, object>>>(),
                false,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        _imageService
            .Setup(x => x.UploadAsync(It.IsAny<IFormFile>(), "recipes/main", It.IsAny<CancellationToken>()))
            .ReturnsAsync(("https://cdn.example.com/new-main.jpg", "new-main-public-id"));
        _imageService
            .Setup(x => x.UploadAsync(It.IsAny<IFormFile>(), "recipes/steps", It.IsAny<CancellationToken>()))
            .ReturnsAsync(("https://cdn.example.com/new-step.jpg", "new-step-public-id"));

        Recipe? capturedRecipe = null;
        _recipeRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()))
            .Callback<Recipe, CancellationToken>((updated, _) => capturedRecipe = updated)
            .ReturnsAsync((Recipe updated, CancellationToken _) => updated);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(RecipeMessages.RecipeUpdatedSuccessfully);
        capturedRecipe.Should().NotBeNull();
        capturedRecipe!.Title.Should().Be("Yeni Tarif");
        capturedRecipe.Description.Should().Be("Güncel açıklama");
        capturedRecipe.Visibility.Should().Be(RecipeVisibility.Private);
        capturedRecipe.Status.Should().Be(RecipeStatus.Published);
        capturedRecipe.RecipeIngredients.Should().ContainSingle(i => i.IngredientId == ingredientId && i.Amount == 3 && i.UnitId == 1);
        capturedRecipe.RecipeSteps.Should().ContainSingle(s => s.StepNumber == 1 && s.Instruction == "Yeni Adım 1");
        _imageService.Verify(x => x.DeleteAsync("old-main-public-id", It.IsAny<CancellationToken>()), Times.Once);
        _imageService.Verify(x => x.DeleteAsync("old-step-1-public-id", It.IsAny<CancellationToken>()), Times.Once);
        _imageService.Verify(x => x.DeleteAsync("old-step-2-public-id", It.IsAny<CancellationToken>()), Times.Once);
        _imageService.Verify(x => x.UploadAsync(It.IsAny<IFormFile>(), "recipes/main", It.IsAny<CancellationToken>()), Times.Once);
        _imageService.Verify(x => x.UploadAsync(It.IsAny<IFormFile>(), "recipes/steps", It.IsAny<CancellationToken>()), Times.Once);
        _recipeRepository.Verify(x => x.UpdateAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private void SetupHttpContext(Guid userId)
    {
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(RecipeTestHelpers.CreateHttpContext(userId));
    }

    private void SetupIngredientCount(UpdateRecipeCommand command)
    {
        _ingredientRepository
            .Setup(x => x.GetCountAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(command.Ingredients.Count);
    }

    private static Recipe BuildExistingRecipe(Guid ownerId, Guid ingredientId)
    {
        var recipe = Recipe.Create(ownerId, "Eski Tarif", "Eski açıklama", RecipeVisibility.Private);
        recipe.AddIngredient(ingredientId, 2, 1);
        recipe.AddStep(1, "Eski Adım 1");
        recipe.AddStep(2, "Eski Adım 2");
        recipe.AddImage("https://cdn.example.com/old-main.jpg", "old-main-public-id", true);

        var step1Id = recipe.RecipeSteps.First(s => s.StepNumber == 1).Id;
        var step2Id = recipe.RecipeSteps.First(s => s.StepNumber == 2).Id;
        recipe.AddImage("https://cdn.example.com/old-step-1.jpg", "old-step-1-public-id", false, step1Id);
        recipe.AddImage("https://cdn.example.com/old-step-2.jpg", "old-step-2-public-id", false, step2Id);

        return recipe;
    }

    private static UpdateRecipeCommand BuildUpdateCommand(Guid recipeId, Guid ingredientId)
    {
        return new UpdateRecipeCommand
        {
            Request = new UpdateRecipeRequest
            {
                Id = recipeId,
                Title = "Yeni Tarif",
                Description = "Güncel açıklama",
                Visibility = RecipeVisibility.Private,
                NewMainImage = RecipeTestHelpers.CreateFormFile("new-main", "new-main.jpg")
            },
            Ingredients =
            [
                new CreateRecipeIngredientRequest { IngredientId = ingredientId, Amount = 3, UnitId = 1 }
            ],
            Steps =
            [
                new UpdateRecipeStepRequest
                {
                    StepNumber = 1,
                    Instruction = "Yeni Adım 1",
                    NewStepImage = RecipeTestHelpers.CreateFormFile("new-step", "new-step.jpg")
                }
            ]
        };
    }
}

public class DeleteRecipeCommandHandlerTests
{
    private readonly Mock<IRecipeRepository> _recipeRepository = new();
    private readonly Mock<IImageService> _imageService = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();
    private readonly Mock<IIngredientRepository> _ingredientRepository = new();
    private readonly Mock<IRecipeCommentRepository> _commentRepository = new();
    private readonly RecipeBusinessRules _rules;
    private readonly DeleteRecipeCommandHandler _handler;

    public DeleteRecipeCommandHandlerTests()
    {
        _rules = new RecipeBusinessRules(_ingredientRepository.Object, _commentRepository.Object);
        _handler = new DeleteRecipeCommandHandler(_recipeRepository.Object, _rules, _imageService.Object, _httpContextAccessor.Object);
    }

    [Fact]
    public async Task Handle_WhenRecipeExists_ShouldDeleteRecipeAndAllImages()
    {
        var userId = Guid.NewGuid();
        var recipe = Recipe.Create(userId, "Tarif", "Açıklama", RecipeVisibility.Private);
        recipe.AddImage("https://cdn.example.com/main.jpg", "main-public-id", true);
        recipe.AddImage("https://cdn.example.com/step.jpg", "step-public-id", false, null);

        _httpContextAccessor.Setup(x => x.HttpContext).Returns(RecipeTestHelpers.CreateHttpContext(userId));
        _recipeRepository
            .Setup(x => x.GetAsync(
                It.IsAny<Expression<Func<Recipe, bool>>>(),
                It.IsAny<Func<IQueryable<Recipe>, IIncludableQueryable<Recipe, object>>>(),
                false,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);
        _imageService.Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _handler.Handle(new DeleteRecipeCommand { Id = recipe.Id }, CancellationToken.None);

        result.Should().Be(RecipeMessages.RecipeDeletedWithRelatedContent);
        _imageService.Verify(x => x.DeleteAsync("main-public-id", It.IsAny<CancellationToken>()), Times.Once);
        _imageService.Verify(x => x.DeleteAsync("step-public-id", It.IsAny<CancellationToken>()), Times.Once);
        _recipeRepository.Verify(x => x.DeleteAsync(recipe, true, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserClaimIsMissing_ShouldThrowArgumentNullException()
    {
        var recipe = Recipe.Create(Guid.NewGuid(), "Tarif", "Açıklama", RecipeVisibility.Private);
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());
        _recipeRepository
            .Setup(x => x.GetAsync(
                It.IsAny<Expression<Func<Recipe, bool>>>(),
                It.IsAny<Func<IQueryable<Recipe>, IIncludableQueryable<Recipe, object>>>(),
                false,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        Func<Task> act = () => _handler.Handle(new DeleteRecipeCommand { Id = recipe.Id }, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}

public class ApproveRecipeCommandHandlerTests
{
    private readonly Mock<IRecipeRepository> _recipeRepository = new();
    private readonly Mock<IIngredientRepository> _ingredientRepository = new();
    private readonly Mock<IRecipeCommentRepository> _commentRepository = new();
    private readonly RecipeBusinessRules _rules;
    private readonly ApproveRecipeCommandHandler _handler;

    public ApproveRecipeCommandHandlerTests()
    {
        _rules = new RecipeBusinessRules(_ingredientRepository.Object, _commentRepository.Object);
        _handler = new ApproveRecipeCommandHandler(_recipeRepository.Object, _rules);
    }

    [Fact]
    public async Task Handle_WhenRecipeIsPendingAdminApproval_ShouldPublishRecipe()
    {
        var recipe = Recipe.Create(Guid.NewGuid(), "Tarif", "Açıklama", RecipeVisibility.Public);
        recipe.PassLlmFilter();

        _recipeRepository
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<Recipe, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        Recipe? updatedRecipe = null;
        _recipeRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()))
            .Callback<Recipe, CancellationToken>((r, _) => updatedRecipe = r)
            .ReturnsAsync((Recipe r, CancellationToken _) => r);

        var result = await _handler.Handle(new ApproveRecipeCommand { Id = recipe.Id }, CancellationToken.None);

        result.Should().Be(RecipeMessages.RecipeApprovedAndPublished);
        updatedRecipe.Should().NotBeNull();
        updatedRecipe!.Status.Should().Be(RecipeStatus.Published);
        _recipeRepository.Verify(x => x.UpdateAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRecipeDoesNotExist_ShouldThrowNotFoundException()
    {
        _recipeRepository
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<Recipe, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Recipe?)null);

        Func<Task> act = () => _handler.Handle(new ApproveRecipeCommand { Id = Guid.NewGuid() }, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(RecipeMessages.RecipeNotFoundOrDeleted);
    }
}

public class RejectRecipeCommandHandlerTests
{
    private readonly Mock<IRecipeRepository> _recipeRepository = new();
    private readonly Mock<IIngredientRepository> _ingredientRepository = new();
    private readonly Mock<IRecipeCommentRepository> _commentRepository = new();
    private readonly RecipeBusinessRules _rules;
    private readonly RejectRecipeCommandHandler _handler;

    public RejectRecipeCommandHandlerTests()
    {
        _rules = new RecipeBusinessRules(_ingredientRepository.Object, _commentRepository.Object);
        _handler = new RejectRecipeCommandHandler(_recipeRepository.Object, _rules);
    }

    [Fact]
    public async Task Handle_WhenRecipeExists_ShouldRejectRecipeWithReason()
    {
        var recipe = Recipe.Create(Guid.NewGuid(), "Tarif", "Açıklama", RecipeVisibility.Public);
        recipe.PassLlmFilter();

        _recipeRepository
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<Recipe, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        Recipe? updatedRecipe = null;
        _recipeRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()))
            .Callback<Recipe, CancellationToken>((r, _) => updatedRecipe = r)
            .ReturnsAsync((Recipe r, CancellationToken _) => r);

        var result = await _handler.Handle(new RejectRecipeCommand { Id = recipe.Id, Reason = "Spam içerik" }, CancellationToken.None);

        result.Should().Be("Tarif reddedildi. Sebep: Spam içerik");
        updatedRecipe.Should().NotBeNull();
        updatedRecipe!.Status.Should().Be(RecipeStatus.Rejected);
        _recipeRepository.Verify(x => x.UpdateAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRecipeDoesNotExist_ShouldThrowNotFoundException()
    {
        _recipeRepository
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<Recipe, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Recipe?)null);

        Func<Task> act = () => _handler.Handle(new RejectRecipeCommand { Id = Guid.NewGuid(), Reason = "Spam" }, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(RecipeMessages.RecipeNotFoundOrDeleted);
    }
}

public class ChangeRecipeVisibilityCommandHandlerTests
{
    private readonly Mock<IRecipeRepository> _recipeRepository = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();
    private readonly Mock<ILocalLlmService> _localLlmService = new();
    private readonly Mock<IIngredientRepository> _ingredientRepository = new();
    private readonly Mock<IRecipeCommentRepository> _commentRepository = new();
    private readonly RecipeBusinessRules _rules;
    private readonly ChangeRecipeVisibilityCommandHandler _handler;

    public ChangeRecipeVisibilityCommandHandlerTests()
    {
        _rules = new RecipeBusinessRules(_ingredientRepository.Object, _commentRepository.Object);
        _handler = new ChangeRecipeVisibilityCommandHandler(_recipeRepository.Object, _rules, _httpContextAccessor.Object, _localLlmService.Object);
    }

    [Fact]
    public async Task Handle_WhenVisibilityIsAlreadyTheSame_ShouldReturnSameVisibilityMessage()
    {
        var userId = Guid.NewGuid();
        var recipe = Recipe.Create(userId, "Tarif", "Açıklama", RecipeVisibility.Private);
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(RecipeTestHelpers.CreateHttpContext(userId));
        _recipeRepository
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<Recipe, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        var result = await _handler.Handle(new ChangeRecipeVisibilityCommand { Id = recipe.Id, Visibility = RecipeVisibility.Private }, CancellationToken.None);

        result.Should().Be(RecipeMessages.VisibilityStatusIsAlreadySame);
        _recipeRepository.Verify(x => x.UpdateAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()), Times.Never);
        _localLlmService.Verify(x => x.CheckRecipeContentForSpamAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenVisibilityChangesToPublicAndContentIsClean_ShouldQueueForReview()
    {
        var userId = Guid.NewGuid();
        var recipe = Recipe.Create(userId, "Tarif", "Açıklama", RecipeVisibility.Private);
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(RecipeTestHelpers.CreateHttpContext(userId));
        _recipeRepository
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<Recipe, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);
        _localLlmService
            .Setup(x => x.CheckRecipeContentForSpamAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        Recipe? updatedRecipe = null;
        _recipeRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()))
            .Callback<Recipe, CancellationToken>((r, _) => updatedRecipe = r)
            .ReturnsAsync((Recipe r, CancellationToken _) => r);

        var result = await _handler.Handle(new ChangeRecipeVisibilityCommand { Id = recipe.Id, Visibility = RecipeVisibility.Public }, CancellationToken.None);

        result.Should().Be(RecipeMessages.RecipePublishedAndQueuedForReview);
        updatedRecipe.Should().NotBeNull();
        updatedRecipe!.Visibility.Should().Be(RecipeVisibility.Public);
        updatedRecipe.Status.Should().Be(RecipeStatus.PendingAdminApproval);
        _recipeRepository.Verify(x => x.UpdateAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()), Times.Once);
        _localLlmService.Verify(x => x.CheckRecipeContentForSpamAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenVisibilityChangesToPublicAndContentIsRejected_ShouldReturnFailureMessage()
    {
        var userId = Guid.NewGuid();
        var recipe = Recipe.Create(userId, "Tarif", "Açıklama", RecipeVisibility.Private);
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(RecipeTestHelpers.CreateHttpContext(userId));
        _recipeRepository
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<Recipe, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);
        _localLlmService
            .Setup(x => x.CheckRecipeContentForSpamAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Recipe? updatedRecipe = null;
        _recipeRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()))
            .Callback<Recipe, CancellationToken>((r, _) => updatedRecipe = r)
            .ReturnsAsync((Recipe r, CancellationToken _) => r);

        var result = await _handler.Handle(new ChangeRecipeVisibilityCommand { Id = recipe.Id, Visibility = RecipeVisibility.Public }, CancellationToken.None);

        result.Should().Be(RecipeMessages.VisibilityUpdateFailedStandards);
        updatedRecipe.Should().NotBeNull();
        updatedRecipe!.Status.Should().Be(RecipeStatus.Rejected);
        _recipeRepository.Verify(x => x.UpdateAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserClaimIsMissing_ShouldThrowUnauthorizedAccessException()
    {
        var recipe = Recipe.Create(Guid.NewGuid(), "Tarif", "Açıklama", RecipeVisibility.Private);
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());
        _recipeRepository
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<Recipe, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        Func<Task> act = () => _handler.Handle(new ChangeRecipeVisibilityCommand { Id = recipe.Id, Visibility = RecipeVisibility.Public }, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}

public class StartCookingCommandHandlerTests
{
    private readonly Mock<IRecipeRepository> _recipeRepository = new();
    private readonly Mock<IUserIngredientRepository> _userIngredientRepository = new();
    private readonly Mock<IShoppingListItemRepository> _shoppingListItemRepository = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();
    private readonly Mock<IIngredientRepository> _ingredientRepository = new();
    private readonly Mock<IRecipeCommentRepository> _commentRepository = new();
    private readonly RecipeBusinessRules _rules;
    private readonly StartCookingCommandHandler _handler;

    public StartCookingCommandHandlerTests()
    {
        _rules = new RecipeBusinessRules(_ingredientRepository.Object, _commentRepository.Object);
        _handler = new StartCookingCommandHandler(_recipeRepository.Object, _userIngredientRepository.Object, _shoppingListItemRepository.Object, _httpContextAccessor.Object, _rules);
    }

    [Fact]
    public async Task Handle_WhenPantryHasEnoughIngredients_ShouldUpdateInventoryOnly()
    {
        var userId = Guid.NewGuid();
        var ingredientId = Guid.NewGuid();
        var recipe = Recipe.Create(userId, "Tarif", "Açıklama", RecipeVisibility.Private);
        recipe.AddIngredient(ingredientId, 2, 1);

        var pantryItem = UserIngredient.CreateStandard(Guid.NewGuid(), userId, ingredientId, 5, MeasurementUnit.Gram);
        var pantryResult = new Paginate<UserIngredient> { Items = [pantryItem], PageNumber = 1, PageSize = 10, TotalItems = 1, TotalPages = 1 };

        _httpContextAccessor.Setup(x => x.HttpContext).Returns(RecipeTestHelpers.CreateHttpContext(userId));
        _recipeRepository
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<Recipe, bool>>>(), It.IsAny<Func<IQueryable<Recipe>, IIncludableQueryable<Recipe, object>>>(), false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);
        _userIngredientRepository
            .Setup(x => x.GetListAsync(It.IsAny<Core.Repositories.Options.QueryOptions<UserIngredient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pantryResult);
        _userIngredientRepository.Setup(x => x.UpdateAsync(It.IsAny<UserIngredient>(), It.IsAny<CancellationToken>())).ReturnsAsync(pantryItem);

        var result = await _handler.Handle(new StartCookingCommand { RecipeId = recipe.Id }, CancellationToken.None);

        result.Should().Be(RecipeMessages.EnjoyAndIngredientsRemovedFromInventory);
        pantryItem.Amount.Should().Be(3);
        _userIngredientRepository.Verify(x => x.UpdateAsync(pantryItem, It.IsAny<CancellationToken>()), Times.Once);
        _shoppingListItemRepository.Verify(x => x.AddAsync(It.IsAny<ShoppingListItem>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenIngredientsRunOut_ShouldAddMissingItemsToShoppingList()
    {
        var userId = Guid.NewGuid();
        var ingredientId = Guid.NewGuid();
        var recipe = Recipe.Create(userId, "Tarif", "Açıklama", RecipeVisibility.Private);
        recipe.AddIngredient(ingredientId, 2, 1);

        var pantryItem = UserIngredient.CreateStandard(Guid.NewGuid(), userId, ingredientId, 1, MeasurementUnit.Gram);
        var pantryResult = new Paginate<UserIngredient> { Items = [pantryItem], PageNumber = 1, PageSize = 10, TotalItems = 1, TotalPages = 1 };
        var shoppingResult = new Paginate<ShoppingListItem> { Items = [], PageNumber = 1, PageSize = 10, TotalItems = 0, TotalPages = 0 };

        _httpContextAccessor.Setup(x => x.HttpContext).Returns(RecipeTestHelpers.CreateHttpContext(userId));
        _recipeRepository
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<Recipe, bool>>>(), It.IsAny<Func<IQueryable<Recipe>, IIncludableQueryable<Recipe, object>>>(), false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);
        _userIngredientRepository
            .Setup(x => x.GetListAsync(It.IsAny<Core.Repositories.Options.QueryOptions<UserIngredient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pantryResult);
        _shoppingListItemRepository
            .Setup(x => x.GetListAsync(It.IsAny<Core.Repositories.Options.QueryOptions<ShoppingListItem>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingResult);
        _userIngredientRepository.Setup(x => x.UpdateAsync(It.IsAny<UserIngredient>(), It.IsAny<CancellationToken>())).ReturnsAsync(pantryItem);
        _shoppingListItemRepository.Setup(x => x.AddAsync(It.IsAny<ShoppingListItem>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ShoppingListItem());

        var result = await _handler.Handle(new StartCookingCommand { RecipeId = recipe.Id }, CancellationToken.None);

        result.Should().Be(RecipeMessages.EnjoyAndInventoryUpdatedWithShoppingList);
        pantryItem.Amount.Should().Be(0);
        _shoppingListItemRepository.Verify(x => x.AddAsync(It.Is<ShoppingListItem>(item => item.UserId == userId && item.IngredientId == ingredientId && !item.IsPurchased), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserClaimIsMissing_ShouldThrowUnauthorizedAccessException()
    {
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

        Func<Task> act = () => _handler.Handle(new StartCookingCommand { RecipeId = Guid.NewGuid() }, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}

public class ToggleInteractionCommandHandlerTests
{
    private readonly Mock<IRecipeRepository> _recipeRepository = new();
    private readonly Mock<IRecipeInteractionRepository> _interactionRepository = new();
    private readonly Mock<IUserProfileVectorService> _vectorService = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();
    private readonly Mock<IIngredientRepository> _ingredientRepository = new();
    private readonly Mock<IRecipeCommentRepository> _commentRepository = new();
    private readonly RecipeBusinessRules _rules;
    private readonly ToggleInteractionCommandHandler _handler;

    public ToggleInteractionCommandHandlerTests()
    {
        _rules = new RecipeBusinessRules(_ingredientRepository.Object, _commentRepository.Object);
        _handler = new ToggleInteractionCommandHandler(_recipeRepository.Object, _interactionRepository.Object, _rules, _vectorService.Object, _httpContextAccessor.Object);
    }

    [Fact]
    public async Task Handle_WhenLikeIsToggled_ShouldCreateInteractionAndUpdateVector()
    {
        var userId = Guid.NewGuid();
        var recipe = Recipe.Create(Guid.NewGuid(), "Tarif", "Açıklama", RecipeVisibility.Public);
        recipe.PassLlmFilter();
        typeof(Recipe).GetProperty(nameof(Recipe.Status))!.SetValue(recipe, RecipeStatus.Published);

        _httpContextAccessor.Setup(x => x.HttpContext).Returns(RecipeTestHelpers.CreateHttpContext(userId));
        _recipeRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Recipe, bool>>>(), null, false, true, It.IsAny<CancellationToken>())).ReturnsAsync(recipe);
        _interactionRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<RecipeInteraction, bool>>>(), null, false, true, It.IsAny<CancellationToken>())).ReturnsAsync((RecipeInteraction?)null);
        _interactionRepository.Setup(x => x.AddAsync(It.IsAny<RecipeInteraction>(), It.IsAny<CancellationToken>())).ReturnsAsync((RecipeInteraction interaction, CancellationToken _) => interaction);
        _interactionRepository.Setup(x => x.UpdateAsync(It.IsAny<RecipeInteraction>(), It.IsAny<CancellationToken>())).ReturnsAsync((RecipeInteraction interaction, CancellationToken _) => interaction);
        _vectorService.Setup(x => x.UpdateProfileVectorAsync(userId, recipe.Id, "Like", It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        RecipeInteraction? capturedInteraction = null;
        _interactionRepository
            .Setup(x => x.AddAsync(It.IsAny<RecipeInteraction>(), It.IsAny<CancellationToken>()))
            .Callback<RecipeInteraction, CancellationToken>((interaction, _) => capturedInteraction = interaction)
            .ReturnsAsync((RecipeInteraction interaction, CancellationToken _) => interaction);

        var result = await _handler.Handle(new ToggleInteractionCommand { RecipeId = recipe.Id, InteractionType = "Like" }, CancellationToken.None);

        result.Should().Be("Like işlemi başarıyla gerçekleştirildi.");
        capturedInteraction.Should().NotBeNull();
        capturedInteraction!.RecipeId.Should().Be(recipe.Id);
        capturedInteraction.UserId.Should().Be(userId);
        capturedInteraction.IsLiked.Should().BeTrue();
        capturedInteraction.IsDisliked.Should().BeFalse();
        _interactionRepository.Verify(x => x.UpdateAsync(It.Is<RecipeInteraction>(i => i.IsLiked && !i.IsDisliked), It.IsAny<CancellationToken>()), Times.Once);
        _vectorService.Verify(x => x.UpdateProfileVectorAsync(userId, recipe.Id, "Like", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenInteractionTypeIsInvalid_ShouldThrowBusinessException()
    {
        var userId = Guid.NewGuid();
        var recipe = Recipe.Create(Guid.NewGuid(), "Tarif", "Açıklama", RecipeVisibility.Public);
        recipe.PassLlmFilter();
        typeof(Recipe).GetProperty(nameof(Recipe.Status))!.SetValue(recipe, RecipeStatus.Published);

        _httpContextAccessor.Setup(x => x.HttpContext).Returns(RecipeTestHelpers.CreateHttpContext(userId));
        _recipeRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Recipe, bool>>>(), null, false, true, It.IsAny<CancellationToken>())).ReturnsAsync(recipe);

        Func<Task> act = () => _handler.Handle(new ToggleInteractionCommand { RecipeId = recipe.Id, InteractionType = "Smile" }, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("Geçersiz etkileşim türü. Geçerli değerler: Like, Dislike, Save");
    }

    [Fact]
    public async Task Handle_WhenInteractionAlreadyExists_ShouldToggleExistingInteraction()
    {
        var userId = Guid.NewGuid();
        var recipe = Recipe.Create(Guid.NewGuid(), "Tarif", "Açıklama", RecipeVisibility.Public);
        recipe.PassLlmFilter();
        typeof(Recipe).GetProperty(nameof(Recipe.Status))!.SetValue(recipe, RecipeStatus.Published);

        var interaction = new RecipeInteraction(Guid.NewGuid(), recipe.Id, userId);

        _httpContextAccessor.Setup(x => x.HttpContext).Returns(RecipeTestHelpers.CreateHttpContext(userId));
        _recipeRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Recipe, bool>>>(), null, false, true, It.IsAny<CancellationToken>())).ReturnsAsync(recipe);
        _interactionRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<RecipeInteraction, bool>>>(), null, false, true, It.IsAny<CancellationToken>())).ReturnsAsync(interaction);
        _interactionRepository.Setup(x => x.UpdateAsync(It.IsAny<RecipeInteraction>(), It.IsAny<CancellationToken>())).ReturnsAsync((RecipeInteraction item, CancellationToken _) => item);
        _vectorService.Setup(x => x.UpdateProfileVectorAsync(userId, recipe.Id, "Dislike", It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _handler.Handle(new ToggleInteractionCommand { RecipeId = recipe.Id, InteractionType = "Dislike" }, CancellationToken.None);

        result.Should().Be("Dislike işlemi başarıyla gerçekleştirildi.");
        interaction.IsDisliked.Should().BeTrue();
        interaction.IsLiked.Should().BeFalse();
        _interactionRepository.Verify(x => x.AddAsync(It.IsAny<RecipeInteraction>(), It.IsAny<CancellationToken>()), Times.Never);
        _interactionRepository.Verify(x => x.UpdateAsync(interaction, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserClaimIsMissing_ShouldThrowArgumentNullException()
    {
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

        Func<Task> act = () => _handler.Handle(new ToggleInteractionCommand { RecipeId = Guid.NewGuid(), InteractionType = "Like" }, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}

public class AddCommentCommandHandlerTests
{
    private readonly Mock<IRecipeRepository> _recipeRepository = new();
    private readonly Mock<IRecipeCommentRepository> _commentRepository = new();
    private readonly Mock<IUserProfileVectorService> _vectorService = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();
    private readonly Mock<IIngredientRepository> _ingredientRepository = new();
    private readonly Mock<IRecipeCommentRepository> _businessCommentRepository = new();
    private readonly RecipeBusinessRules _rules;
    private readonly AddCommentCommandHandler _handler;

    public AddCommentCommandHandlerTests()
    {
        _rules = new RecipeBusinessRules(_ingredientRepository.Object, _businessCommentRepository.Object);
        _handler = new AddCommentCommandHandler(_recipeRepository.Object, _commentRepository.Object, _rules, _vectorService.Object, _httpContextAccessor.Object);
    }

    [Fact]
    public async Task Handle_WhenRootCommentIsAdded_ShouldTrimContentAndReturnSuccessMessage()
    {
        var userId = Guid.NewGuid();
        var recipe = Recipe.Create(userId, "Tarif", "Açıklama", RecipeVisibility.Public);
        recipe.PassLlmFilter();
        typeof(Recipe).GetProperty(nameof(Recipe.Status))!.SetValue(recipe, RecipeStatus.Published);

        _httpContextAccessor.Setup(x => x.HttpContext).Returns(RecipeTestHelpers.CreateHttpContext(userId));
        _recipeRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Recipe, bool>>>(), null, false, true, It.IsAny<CancellationToken>())).ReturnsAsync(recipe);
        _businessCommentRepository.Setup(x => x.CheckIfParentCommentExistsAsync(It.IsAny<Guid?>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        RecipeComment? capturedComment = null;
        _commentRepository
            .Setup(x => x.AddAsync(It.IsAny<RecipeComment>(), It.IsAny<CancellationToken>()))
            .Callback<RecipeComment, CancellationToken>((comment, _) => capturedComment = comment)
            .ReturnsAsync((RecipeComment comment, CancellationToken _) => comment);
        _vectorService.Setup(x => x.UpdateProfileVectorAsync(userId, recipe.Id, "Comment", It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _handler.Handle(new AddCommentCommand { RecipeId = recipe.Id, Content = "  Harika tarif!  " }, CancellationToken.None);

        result.Should().Be("Yorumunuz başarıyla eklendi.");
        capturedComment.Should().NotBeNull();
        capturedComment!.Content.Should().Be("Harika tarif!");
        capturedComment.ParentCommentId.Should().BeNull();
        _commentRepository.Verify(x => x.AddAsync(It.IsAny<RecipeComment>(), It.IsAny<CancellationToken>()), Times.Once);
        _vectorService.Verify(x => x.UpdateProfileVectorAsync(userId, recipe.Id, "Comment", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenReplyIsAdded_ShouldReturnReplyMessage()
    {
        var userId = Guid.NewGuid();
        var parentCommentId = Guid.NewGuid();
        var recipe = Recipe.Create(userId, "Tarif", "Açıklama", RecipeVisibility.Public);
        recipe.PassLlmFilter();
        typeof(Recipe).GetProperty(nameof(Recipe.Status))!.SetValue(recipe, RecipeStatus.Published);

        _httpContextAccessor.Setup(x => x.HttpContext).Returns(RecipeTestHelpers.CreateHttpContext(userId));
        _recipeRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Recipe, bool>>>(), null, false, true, It.IsAny<CancellationToken>())).ReturnsAsync(recipe);
        _businessCommentRepository.Setup(x => x.CheckIfParentCommentExistsAsync(parentCommentId, recipe.Id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _commentRepository.Setup(x => x.AddAsync(It.IsAny<RecipeComment>(), It.IsAny<CancellationToken>())).ReturnsAsync((RecipeComment comment, CancellationToken _) => comment);
        _vectorService.Setup(x => x.UpdateProfileVectorAsync(userId, recipe.Id, "Comment", It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _handler.Handle(new AddCommentCommand { RecipeId = recipe.Id, ParentCommentId = parentCommentId, Content = "  Teşekkürler  " }, CancellationToken.None);

        result.Should().Be("Cevabınız başarıyla eklendi.");
    }

    [Fact]
    public async Task Handle_WhenUserClaimIsMissing_ShouldThrowArgumentNullException()
    {
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

        Func<Task> act = () => _handler.Handle(new AddCommentCommand { RecipeId = Guid.NewGuid(), Content = "yorum" }, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}

public class UpdateCommentCommandHandlerTests
{
    private readonly Mock<IRecipeCommentRepository> _commentRepository = new();
    private readonly Mock<IUserProfileVectorService> _vectorService = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();
    private readonly Mock<IIngredientRepository> _ingredientRepository = new();
    private readonly Mock<IRecipeCommentRepository> _businessCommentRepository = new();
    private readonly RecipeBusinessRules _rules;
    private readonly UpdateCommentCommandHandler _handler;

    public UpdateCommentCommandHandlerTests()
    {
        _rules = new RecipeBusinessRules(_ingredientRepository.Object, _businessCommentRepository.Object);
        _handler = new UpdateCommentCommandHandler(_commentRepository.Object, _rules, _vectorService.Object, _httpContextAccessor.Object);
    }

    [Fact]
    public async Task Handle_WhenCommentBelongsToUser_ShouldUpdateContent()
    {
        var userId = Guid.NewGuid();
        var comment = new RecipeComment(Guid.NewGuid(), Guid.NewGuid(), userId, "Eski içerik");

        _httpContextAccessor.Setup(x => x.HttpContext).Returns(RecipeTestHelpers.CreateHttpContext(userId));
        _commentRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<RecipeComment, bool>>>(), null, false, true, It.IsAny<CancellationToken>())).ReturnsAsync(comment);
        _commentRepository.Setup(x => x.UpdateAsync(It.IsAny<RecipeComment>(), It.IsAny<CancellationToken>())).ReturnsAsync((RecipeComment item, CancellationToken _) => item);
        _vectorService.Setup(x => x.UpdateProfileVectorAsync(userId, comment.RecipeId, "Update_Comment", It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _handler.Handle(new UpdateCommentCommand { CommentId = comment.Id, NewContent = "  Güncel içerik  " }, CancellationToken.None);

        result.Should().Be(RecipeMessages.CommentUpdatedSuccessfullyMessage);
        comment.Content.Should().Be("  Güncel içerik  ");
        comment.UpdatedDate.Should().NotBeNull();
        _commentRepository.Verify(x => x.UpdateAsync(comment, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCommentBelongsToAnotherUser_ShouldThrowAuthorizationException()
    {
        var userId = Guid.NewGuid();
        var comment = new RecipeComment(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Eski içerik");

        _httpContextAccessor.Setup(x => x.HttpContext).Returns(RecipeTestHelpers.CreateHttpContext(userId));
        _commentRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<RecipeComment, bool>>>(), null, false, true, It.IsAny<CancellationToken>())).ReturnsAsync(comment);

        Func<Task> act = () => _handler.Handle(new UpdateCommentCommand { CommentId = comment.Id, NewContent = "Yeni içerik" }, CancellationToken.None);

        await act.Should().ThrowAsync<AuthorizationException>()
            .WithMessage(RecipeMessages.UnauthorizedCommentAction);
    }
}

public class DeleteCommentCommandHandlerTests
{
    private readonly Mock<IRecipeCommentRepository> _commentRepository = new();
    private readonly Mock<IUserProfileVectorService> _vectorService = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();
    private readonly Mock<IIngredientRepository> _ingredientRepository = new();
    private readonly Mock<IRecipeCommentRepository> _businessCommentRepository = new();
    private readonly RecipeBusinessRules _rules;
    private readonly DeleteCommentCommandHandler _handler;

    public DeleteCommentCommandHandlerTests()
    {
        _rules = new RecipeBusinessRules(_ingredientRepository.Object, _businessCommentRepository.Object);
        _handler = new DeleteCommentCommandHandler(_commentRepository.Object, _rules, _vectorService.Object, _httpContextAccessor.Object);
    }

    [Fact]
    public async Task Handle_WhenCommentBelongsToUser_ShouldDeleteComment()
    {
        var userId = Guid.NewGuid();
        var comment = new RecipeComment(Guid.NewGuid(), Guid.NewGuid(), userId, "Silinecek yorum");

        _httpContextAccessor.Setup(x => x.HttpContext).Returns(RecipeTestHelpers.CreateHttpContext(userId));
        _commentRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<RecipeComment, bool>>>(), null, false, true, It.IsAny<CancellationToken>())).ReturnsAsync(comment);
        _commentRepository.Setup(x => x.DeleteAsync(It.IsAny<RecipeComment>(), false, It.IsAny<CancellationToken>())).ReturnsAsync(comment);
        _vectorService.Setup(x => x.UpdateProfileVectorAsync(userId, comment.RecipeId, "Delete_Comment", It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _handler.Handle(new DeleteCommentCommand { CommentId = comment.Id }, CancellationToken.None);

        result.Should().Be(RecipeMessages.CommentDeletedSuccessfullyMessage);
        _commentRepository.Verify(x => x.DeleteAsync(comment, false, It.IsAny<CancellationToken>()), Times.Once);
        _vectorService.Verify(x => x.UpdateProfileVectorAsync(userId, comment.RecipeId, "Delete_Comment", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCommentBelongsToAnotherUser_ShouldThrowAuthorizationException()
    {
        var userId = Guid.NewGuid();
        var comment = new RecipeComment(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Silinecek yorum");

        _httpContextAccessor.Setup(x => x.HttpContext).Returns(RecipeTestHelpers.CreateHttpContext(userId));
        _commentRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<RecipeComment, bool>>>(), null, false, true, It.IsAny<CancellationToken>())).ReturnsAsync(comment);

        Func<Task> act = () => _handler.Handle(new DeleteCommentCommand { CommentId = comment.Id }, CancellationToken.None);

        await act.Should().ThrowAsync<AuthorizationException>()
            .WithMessage(RecipeMessages.UnauthorizedCommentAction);
    }
}
