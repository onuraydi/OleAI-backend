using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Pantry.Events;
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.ImageStorage;
using OleAI.Backend.Application.Services.Llm;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Recipes.Commands.UpdateRecipe;

public class UpdateRecipeCommandHandler : IRequestHandler<UpdateRecipeCommand, string>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IIngredientRequestRepository _ingredientRequestRepository;
    private readonly RecipeBusinessRules _recipeBusinessRules;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILocalLlmService _localLlmService;
    private readonly IImageService _imageService;
    private readonly IMediator _mediator;

    public UpdateRecipeCommandHandler(
        IRecipeRepository recipeRepository,
        IIngredientRequestRepository ingredientRequestRepository,
        RecipeBusinessRules recipeBusinessRules,
        IHttpContextAccessor httpContextAccessor,
        ILocalLlmService localLlmService,
        IImageService imageService,
        IMediator mediator)
    {
        _recipeRepository = recipeRepository;
        _ingredientRequestRepository = ingredientRequestRepository;
        _recipeBusinessRules = recipeBusinessRules;
        _httpContextAccessor = httpContextAccessor;
        _localLlmService = localLlmService;
        _imageService = imageService;
        _mediator = mediator;
    }

    public async Task<string> Handle(UpdateRecipeCommand command, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) throw new UnauthorizedAccessException();
        var userId = Guid.Parse(userIdClaim);

        var recipe = await _recipeRepository.GetAsync(
            predicate: r => r.Id == command.Request.Id,
            include: q => q.Include(r => r.RecipeIngredients).Include(r => r.RecipeSteps).Include(r => r.RecipeImages),
            cancellationToken: cancellationToken);

        _recipeBusinessRules.CheckIfRecipeExists(recipe);

        var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;
        if (!isAdmin)
        {
            _recipeBusinessRules.CheckIfUserIsOwner(recipe!, userId);
        }

        var ingredientIds = command.Ingredients.Where(i => i.IngredientId.HasValue).Select(i => i.IngredientId!.Value).ToList();
        await _recipeBusinessRules.CheckIfIngredientsExistAsync(ingredientIds, cancellationToken);

        recipe!.UpdateDetails(command.Request.Title, command.Request.Description, command.Request.Visibility);

        var ingredientTuples = new List<(Guid? IngredientId, Guid? IngredientRequestId, decimal Amount, int UnitId)>();
        var requestsToAdd = new List<IngredientRequest>();
        var eventsToPublish = new List<IngredientRequestedEvent>();

        foreach (var ingredientDto in command.Ingredients)
        {
            if (ingredientDto.IngredientId.HasValue)
            {
                ingredientTuples.Add((ingredientDto.IngredientId.Value, null, ingredientDto.Amount, ingredientDto.UnitId));
            }
            else if (!string.IsNullOrEmpty(ingredientDto.RequestedName))
            {
                var ingredientRequest = new IngredientRequest(Guid.NewGuid(), userId, ingredientDto.RequestedName, ingredientDto.SuggestedCategoryId);
                requestsToAdd.Add(ingredientRequest);

                ingredientTuples.Add((null, ingredientRequest.Id, ingredientDto.Amount, ingredientDto.UnitId));

                eventsToPublish.Add(new IngredientRequestedEvent
                {
                    RequestId = ingredientRequest.Id,
                    RequestedName = ingredientRequest.RequestedName
                });
            }
        }

        if (requestsToAdd.Any())
        {
            await _ingredientRequestRepository.AddRangeAsync(requestsToAdd, cancellationToken);
        }

        recipe.SyncIngredients(ingredientTuples);

        var stepTuples = command.Steps.OrderBy(s => s.StepNumber).Select(s => (s.StepNumber, s.Instruction));
        var orphanedImageIds = recipe.SyncSteps(stepTuples);

        foreach (var publicId in orphanedImageIds)
        {
            await _imageService.DeleteAsync(publicId, cancellationToken);
        }

        if (command.Request.NewMainImage != null)
        {
            var mainUpload = await _imageService.UploadAsync(command.Request.NewMainImage, "recipes/main", cancellationToken);
            string? oldMainPublicId = recipe.ReplaceImage(mainUpload.Url, mainUpload.PublicId, isMain: true);

            if (!string.IsNullOrEmpty(oldMainPublicId))
                await _imageService.DeleteAsync(oldMainPublicId, cancellationToken);
        }

        foreach (var stepDto in command.Steps.Where(s => s.NewStepImage != null))
        {
            var currentStep = recipe.RecipeSteps.First(s => s.StepNumber == stepDto.StepNumber);

            var stepUpload = await _imageService.UploadAsync(stepDto.NewStepImage!, "recipes/steps", cancellationToken);
            string? oldStepPublicId = recipe.ReplaceImage(stepUpload.Url, stepUpload.PublicId, isMain: false, stepId: currentStep.Id);

            if (!string.IsNullOrEmpty(oldStepPublicId))
                await _imageService.DeleteAsync(oldStepPublicId, cancellationToken);
        }

        if (isAdmin)
        {
            if (recipe.Visibility == RecipeVisibility.Public)
            {
                recipe.ForcePublish();
            }
        }
        else if (recipe.Visibility == RecipeVisibility.Public)
        {
            bool isClean = await _localLlmService.CheckRecipeContentForSpamAsync(recipe.Title, recipe.Description, cancellationToken);
            if (isClean) recipe.PassLlmFilter();
            else recipe.Reject(RecipeMessages.SpamDetectedAfterUpdate);
        }

        await _recipeRepository.UpdateAsync(recipe);

        foreach (var evt in eventsToPublish)
        {
            await _mediator.Publish(evt, cancellationToken);
        }

        return recipe.Status == RecipeStatus.Rejected
            ? RecipeMessages.RecipeUpdatedButRejectedDueToSpam
            : RecipeMessages.RecipeUpdatedSuccessfully;
    }
}
