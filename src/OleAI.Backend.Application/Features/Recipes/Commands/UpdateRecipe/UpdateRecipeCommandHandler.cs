using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OleAI.Backend.Application.Features.Pantry.Events;
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.AiRecommendation;
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
    private readonly IAiRecommendationService _aiRecommendationService;
    private readonly IServiceScopeFactory _scopeFactory;

    public UpdateRecipeCommandHandler(
        IRecipeRepository recipeRepository,
        IIngredientRequestRepository ingredientRequestRepository,
        RecipeBusinessRules recipeBusinessRules,
        IHttpContextAccessor httpContextAccessor,
        ILocalLlmService localLlmService,
        IImageService imageService,
        IMediator mediator,
        IAiRecommendationService aiRecommendationService,
        IServiceScopeFactory scopeFactory)
    {
        _recipeRepository = recipeRepository;
        _ingredientRequestRepository = ingredientRequestRepository;
        _recipeBusinessRules = recipeBusinessRules;
        _httpContextAccessor = httpContextAccessor;
        _localLlmService = localLlmService;
        _imageService = imageService;
        _mediator = mediator;
        _aiRecommendationService = aiRecommendationService;
        _scopeFactory = scopeFactory;
    }

    public async Task<string> Handle(UpdateRecipeCommand command, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) throw new UnauthorizedAccessException();
        var userId = Guid.Parse(userIdClaim);

        var recipe = await _recipeRepository.GetAsync(
            predicate: r => r.Id == command.Request.Id,
            include: q => q.Include(r => r.RecipeIngredients)
                           .Include(r => r.RecipeSteps)
                           .Include(r => r.RecipeImages)
                           .Include(r => r.RecipeCategories)
                           .Include(r => r.RecipeTags),
            cancellationToken: cancellationToken);

        _recipeBusinessRules.CheckIfRecipeExists(recipe);

        var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;
        if (!isAdmin)
        {
            _recipeBusinessRules.CheckIfUserIsOwner(recipe!, userId);
        }

        var ingredientIds = command.Ingredients.Where(i => i.IngredientId.HasValue).Select(i => i.IngredientId!.Value).ToList();
        await _recipeBusinessRules.CheckIfIngredientsExistAsync(ingredientIds, cancellationToken);

        recipe!.UpdateDetails(
            command.Request.Title,
            command.Request.Description,
            command.Request.Difficulty,
            command.Request.Servings,
            command.Request.PrepTime,
            command.Request.CookTime,
            command.Request.Visibility
        );

        if (command.Request.CategoryIds.Any())
            recipe.SyncCategories(command.Request.CategoryIds);

        if (command.Request.TagIds.Any())
            recipe.SyncTags(command.Request.TagIds);

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

        await _recipeRepository.UpdateAsync(recipe);

        foreach (var evt in eventsToPublish)
        {
            await _mediator.Publish(evt, cancellationToken);
        }

        if (!isAdmin && recipe.Visibility == RecipeVisibility.Public)
        {
            var recipeId = recipe.Id;
            _ = Task.Run(async () =>
            {
                using var scope = _scopeFactory.CreateScope();
                var llmService = scope.ServiceProvider.GetRequiredService<ILocalLlmService>();
                var recipeRepo = scope.ServiceProvider.GetRequiredService<IRecipeRepository>();
                var aiService = scope.ServiceProvider.GetRequiredService<IAiRecommendationService>();

                try
                {
                    var savedRecipe = await recipeRepo.GetAsync(r => r.Id == recipeId);
                    if (savedRecipe != null && savedRecipe.Status == RecipeStatus.PendingLlmApproval)
                    {
                        bool isClean = await llmService.CheckRecipeContentForSpamAsync(savedRecipe.Title, savedRecipe.Description, CancellationToken.None);
                        if (isClean)
                            savedRecipe.PassLlmFilter();
                        else
                            savedRecipe.Reject(RecipeMessages.SpamDetectedAfterUpdate);

                        await recipeRepo.UpdateAsync(savedRecipe);

                        if (savedRecipe.Status == RecipeStatus.Published)
                        {
                            var updatedRecipeWithIngredients = await recipeRepo.GetAsync(
                                predicate: r => r.Id == savedRecipe.Id,
                                include: q => q.Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient).ThenInclude(i => i.Aliases).Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient).ThenInclude(i => i.Category).Include(r => r.Interactions));

                            if (updatedRecipeWithIngredients != null)
                            {
                                var structuredIngredients = updatedRecipeWithIngredients.RecipeIngredients.Where(ri => ri.Ingredient != null).Select(ri =>
                                {
                                    var baseUnit = ri.Ingredient!.UnitFamily switch
                                    {
                                        OleAI.Backend.Domain.Enums.MeasurementCategory.Weight => OleAI.Backend.Domain.Enums.MeasurementUnit.Gram,
                                        OleAI.Backend.Domain.Enums.MeasurementCategory.Volume => OleAI.Backend.Domain.Enums.MeasurementUnit.Milliliter,
                                        _ => OleAI.Backend.Domain.Enums.MeasurementUnit.Piece
                                    };
                                    return new OleAI.Backend.Application.Services.AiRecommendation.RecipeIngredientAiDto
                                    {
                                        Id = ri.Ingredient.Id,
                                        Name = ri.Ingredient.Name,
                                        Amount = OleAI.Backend.Domain.Services.MeasurementUnitHelper.ConvertAmount((double)ri.Amount, (OleAI.Backend.Domain.Enums.MeasurementUnit)ri.UnitId, baseUnit),
                                        Unit = baseUnit.ToString(),
                                        Aliases = ri.Ingredient.Aliases.Select(a => a.AliasName).ToList()
                                    };
                                }).ToList();

                                var representativeCategory = updatedRecipeWithIngredients.RecipeIngredients.FirstOrDefault(ri => ri.Ingredient?.Category != null)?.Ingredient?.Category?.Name;
                                var popularity = updatedRecipeWithIngredients.Interactions?.Count ?? 0;

                                await aiService.SyncRecipeAsync(
                                    updatedRecipeWithIngredients.Id,
                                    updatedRecipeWithIngredients.Title,
                                    structuredIngredients,
                                    representativeCategory,
                                    isPublished: true,
                                    popularity: popularity,
                                    cancellationToken: CancellationToken.None);
                            }
                        }
                    }
                }
                catch (Exception) { }
            });
        }
        else if (recipe.Status == RecipeStatus.Published)
        {
            var updatedRecipeWithIngredients = await _recipeRepository.GetAsync(
                predicate: r => r.Id == recipe.Id,
                include: q => q.Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient).ThenInclude(i => i.Aliases).Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient).ThenInclude(i => i.Category).Include(r => r.Interactions),
                cancellationToken: cancellationToken);

            if (updatedRecipeWithIngredients != null)
            {
                var structuredIngredients = updatedRecipeWithIngredients.RecipeIngredients.Where(ri => ri.Ingredient != null).Select(ri =>
                {
                    var baseUnit = ri.Ingredient!.UnitFamily switch
                    {
                        OleAI.Backend.Domain.Enums.MeasurementCategory.Weight => OleAI.Backend.Domain.Enums.MeasurementUnit.Gram,
                        OleAI.Backend.Domain.Enums.MeasurementCategory.Volume => OleAI.Backend.Domain.Enums.MeasurementUnit.Milliliter,
                        _ => OleAI.Backend.Domain.Enums.MeasurementUnit.Piece
                    };
                    return new OleAI.Backend.Application.Services.AiRecommendation.RecipeIngredientAiDto
                    {
                        Id = ri.Ingredient.Id,
                        Name = ri.Ingredient.Name,
                        Amount = OleAI.Backend.Domain.Services.MeasurementUnitHelper.ConvertAmount((double)ri.Amount, (OleAI.Backend.Domain.Enums.MeasurementUnit)ri.UnitId, baseUnit),
                        Unit = baseUnit.ToString(),
                        Aliases = ri.Ingredient.Aliases.Select(a => a.AliasName).ToList()
                    };
                }).ToList();

                var representativeCategory = updatedRecipeWithIngredients.RecipeIngredients.FirstOrDefault(ri => ri.Ingredient?.Category != null)?.Ingredient?.Category?.Name;
                var popularity = updatedRecipeWithIngredients.Interactions?.Count ?? 0;

                await _aiRecommendationService.SyncRecipeAsync(
                    updatedRecipeWithIngredients.Id,
                    updatedRecipeWithIngredients.Title,
                    structuredIngredients,
                    representativeCategory,
                    isPublished: true,
                    popularity: popularity,
                    cancellationToken: cancellationToken);
            }
        }

        return recipe.Status == RecipeStatus.Rejected
            ? RecipeMessages.RecipeUpdatedButRejectedDueToSpam
            : RecipeMessages.RecipeUpdatedSuccessfully;
    }
}
