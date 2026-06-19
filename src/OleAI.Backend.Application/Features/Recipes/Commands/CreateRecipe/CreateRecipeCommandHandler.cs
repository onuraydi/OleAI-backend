using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Pantry.Events;
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.AiRecommendation;
using OleAI.Backend.Application.Services.ImageStorage;
using OleAI.Backend.Application.Services.Llm;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Recipes.Commands.CreateRecipe
{
    public class CreateRecipeCommandHandler : IRequestHandler<CreateRecipeCommand, string>
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IIngredientRequestRepository _ingredientRequestRepository;
        private readonly IImageService _imageService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RecipeBusinessRules _recipeBusinessRules;
        private readonly ILocalLlmService _localLlmService;
        private readonly IMediator _mediator;
        private readonly IAiRecommendationService _aiRecommendationService;

        public CreateRecipeCommandHandler(IRecipeRepository recipeRepository, IIngredientRequestRepository ingredientRequestRepository, IImageService imageService, IHttpContextAccessor httpContextAccessor, RecipeBusinessRules recipeBusinessRules, ILocalLlmService localLlmService, IMediator mediator, IAiRecommendationService aiRecommendationService)
        {
            _recipeRepository = recipeRepository;
            _ingredientRequestRepository = ingredientRequestRepository;
            _imageService = imageService;
            _httpContextAccessor = httpContextAccessor;
            _recipeBusinessRules = recipeBusinessRules;
            _localLlmService = localLlmService;
            _mediator = mediator;
            _aiRecommendationService = aiRecommendationService;
        }

        public async Task<string> Handle(CreateRecipeCommand command, CancellationToken cancellationToken)
        {
            var requestedIngredientIds = command.Ingredients.Where(i => i.IngredientId.HasValue).Select(i => i.IngredientId!.Value).Distinct().ToList();
            await _recipeBusinessRules.CheckIfIngredientsExistAsync(requestedIngredientIds, cancellationToken);

            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException(RecipeMessages.UnauthorizedAccess);

            var userId = Guid.Parse(userIdClaim);

            var recipe = Recipe.Create(
                userId,
                command.CreateRecipeRequest.Title,
                command.CreateRecipeRequest.Description,
                command.CreateRecipeRequest.Difficulty,
                command.CreateRecipeRequest.Servings,
                command.CreateRecipeRequest.PrepTime,
                command.CreateRecipeRequest.CookTime,
                command.CreateRecipeRequest.Visibility
            );

            if (command.CreateRecipeRequest.CategoryIds.Any())
                recipe.SyncCategories(command.CreateRecipeRequest.CategoryIds);

            if (command.CreateRecipeRequest.TagIds.Any())
                recipe.SyncTags(command.CreateRecipeRequest.TagIds);

            var mainImageUpload = await _imageService.UploadAsync(command.CreateRecipeRequest.MainImage, "recipes/main", cancellationToken);
            recipe.AddImage(mainImageUpload.Url, mainImageUpload.PublicId, isMain: true);

            bool hasPendingIngredients = false;
            var requestsToAdd = new List<IngredientRequest>();
            var eventsToPublish = new List<IngredientRequestedEvent>();

            foreach (var ingredientDto in command.Ingredients)
            {
                if (ingredientDto.IngredientId.HasValue)
                {
                    recipe.AddIngredient(ingredientDto.IngredientId.Value, null, ingredientDto.Amount, ingredientDto.UnitId);
                }
                else if (!string.IsNullOrEmpty(ingredientDto.RequestedName))
                {
                    hasPendingIngredients = true;
                    var ingredientRequest = new IngredientRequest(Guid.NewGuid(), userId, ingredientDto.RequestedName, ingredientDto.SuggestedCategoryId);
                    requestsToAdd.Add(ingredientRequest);

                    recipe.AddIngredient(null, ingredientRequest.Id, ingredientDto.Amount, ingredientDto.UnitId);

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

            var orderedSteps = command.Steps.OrderBy(s => s.StepNumber).ToList();
            for (int i = 0; i < orderedSteps.Count; i++)
            {
                var stepDto = orderedSteps[i];
                int currentStepNumber = i + 1;

                recipe.AddStep(currentStepNumber, stepDto.Instruction);

                if (stepDto.StepImage != null)
                {
                    var addedStep = recipe.RecipeSteps.FirstOrDefault(s => s.StepNumber == currentStepNumber);
                    if (addedStep != null)
                    {
                        var stepImageUpload = await _imageService.UploadAsync(stepDto.StepImage, "recipes/steps", cancellationToken);
                        recipe.AddImage(stepImageUpload.Url, stepImageUpload.PublicId, isMain: false, stepId: addedStep.Id);
                    }
                }
            }

            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;

            if (isAdmin)
            {
                if (recipe.Visibility == RecipeVisibility.Public)
                {
                    recipe.ForcePublish();
                }
            }
            else if (recipe.Visibility == RecipeVisibility.Public)
            {
                bool isContentClean = await _localLlmService.CheckRecipeContentForSpamAsync(recipe.Title, recipe.Description, cancellationToken);

                if (isContentClean)
                    recipe.PassLlmFilter();
                else
                    recipe.Reject(RecipeMessages.AiModeratorDetectedHarmfulContent);
            }

            await _recipeRepository.AddAsync(recipe);

            foreach (var evt in eventsToPublish)
            {
                await _mediator.Publish(evt, cancellationToken);
            }

            if (recipe.Status == RecipeStatus.Published)
            {
                var createdRecipeWithIngredients = await _recipeRepository.GetAsync(
                    predicate: r => r.Id == recipe.Id,
                    include: q => q.Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient).ThenInclude(i => i.Aliases).Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient).ThenInclude(i => i.Category),
                    cancellationToken: cancellationToken);

                if (createdRecipeWithIngredients != null)
                {
                    var structuredIngredients = createdRecipeWithIngredients.RecipeIngredients.Where(ri => ri.Ingredient != null).Select(ri =>
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

                    var representativeCategory = createdRecipeWithIngredients.RecipeIngredients.FirstOrDefault(ri => ri.Ingredient?.Category != null)?.Ingredient?.Category?.Name;

                    await _aiRecommendationService.SyncRecipeAsync(
                        createdRecipeWithIngredients.Id,
                        createdRecipeWithIngredients.Title,
                        structuredIngredients,
                        representativeCategory,
                        isPublished: true,
                        popularity: 0,
                        cancellationToken: cancellationToken);
                }
            }

            if (recipe.Status == RecipeStatus.Rejected)
                return RecipeMessages.RecipeRejectedDueToSystemStandards;

            if (recipe.Visibility == RecipeVisibility.Private)
                return RecipeMessages.RecipeCreatedAndSavedAsPrivate;

            if (isAdmin)
                return "Tarif başarıyla oluşturuldu ve yayınlandı.";

            return RecipeMessages.RecipeCreatedAndPendingAdminApproval;
        }
    }
}
