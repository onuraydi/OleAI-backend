using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.AiRecommendation;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using OleAI.Backend.Domain.Services;

namespace OleAI.Backend.Application.Features.Recipes.Commands.ApproveRecipe;

public class ApproveRecipeCommandHandler : IRequestHandler<ApproveRecipeCommand, string>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IIngredientRequestRepository _ingredientRequestRepository;
    private readonly RecipeBusinessRules _recipeBusinessRules;
    private readonly IAiRecommendationService _aiRecommendationService;
    private readonly ICacheService _cacheService;

    public ApproveRecipeCommandHandler(
        IRecipeRepository recipeRepository,
        IIngredientRepository ingredientRepository,
        IIngredientRequestRepository ingredientRequestRepository,
        RecipeBusinessRules recipeBusinessRules,
        IAiRecommendationService aiRecommendationService,
        ICacheService cacheService)
    {
        _recipeRepository = recipeRepository;
        _ingredientRepository = ingredientRepository;
        _ingredientRequestRepository = ingredientRequestRepository;
        _recipeBusinessRules = recipeBusinessRules;
        _aiRecommendationService = aiRecommendationService;
        _cacheService = cacheService;
    }

    public async Task<string> Handle(ApproveRecipeCommand request, CancellationToken cancellationToken)
    {
        var recipe = await _recipeRepository.GetAsync(
            predicate: r => r.Id == request.Id,
            include: q => q.Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient).Include(r => r.RecipeSteps),
            cancellationToken: cancellationToken);

        _recipeBusinessRules.CheckIfRecipeExists(recipe);

        recipe!.ApproveByAdmin();

        // Eğer admin manuel olarak düzeltmişse öncelikle onları al
        if (!string.IsNullOrEmpty(request.EditedDescription) && request.EditedSteps != null && request.EditedSteps.Any())
        {
            var incomingSteps = request.EditedSteps.Select(s => (s.StepNumber, s.Instruction)).ToList();
            recipe.ApplyLlmCorrections(request.EditedDescription, incomingSteps);
        }
        else if (!string.IsNullOrEmpty(recipe.LlmReviewResultJson))
        {
            try
            {
                using var jsonDoc = System.Text.Json.JsonDocument.Parse(recipe.LlmReviewResultJson);
                var root = jsonDoc.RootElement;
                if (root.TryGetProperty("isApproved", out var isApprovedProp) && isApprovedProp.GetBoolean())
                {
                    if (root.TryGetProperty("correctedDescription", out var descProp) && descProp.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var newDesc = descProp.GetString()!;

                        var incomingSteps = new List<(int StepNumber, string Instruction)>();
                        if (root.TryGetProperty("correctedSteps", out var stepsProp) && stepsProp.ValueKind == System.Text.Json.JsonValueKind.Array)
                        {
                            int stepNumber = 1;
                            foreach (var stepElement in stepsProp.EnumerateArray())
                            {
                                var instruction = stepElement.GetString();
                                if (!string.IsNullOrWhiteSpace(instruction))
                                {
                                    incomingSteps.Add((stepNumber, instruction));
                                    stepNumber++;
                                }
                            }
                        }

                        recipe.ApplyLlmCorrections(newDesc, incomingSteps);
                    }
                }
            }
            catch (Exception ex)
            {
                // Parse hatası olursa sessizce geç
            }
        }

        var pendingRecipeIngredients = recipe.RecipeIngredients
            .Where(ri => ri.IngredientRequestId.HasValue && !ri.IngredientId.HasValue)
            .ToList();

        if (pendingRecipeIngredients.Any())
        {
            var requestIds = pendingRecipeIngredients.Select(ri => ri.IngredientRequestId!.Value).ToList();
            var query = new QueryOptions<IngredientRequest>();
            query.Predicate = ir => requestIds.Contains(ir.Id);
            var ingredientRequests = await _ingredientRequestRepository.GetListAsync(
                query,
                cancellationToken: cancellationToken);

            var ingredientsToAdd = new List<Ingredient>();
            var requestsToUpdate = new List<IngredientRequest>();

            foreach (var req in ingredientRequests.Items)
            {
                if (req.Status == IngredientRequestStatus.Pending)
                {
                    var recipeIng = pendingRecipeIngredients.First(ri => ri.IngredientRequestId == req.Id);

                    var unit = (MeasurementUnit)recipeIng.UnitId;
                    MeasurementCategory unitFamily = MeasurementCategory.Other;

                    if (MeasurementUnitHelper.GetUnitsForCategory(MeasurementCategory.Weight).Contains(unit))
                        unitFamily = MeasurementCategory.Weight;
                    else if (MeasurementUnitHelper.GetUnitsForCategory(MeasurementCategory.Volume).Contains(unit))
                        unitFamily = MeasurementCategory.Volume;
                    else if (MeasurementUnitHelper.GetUnitsForCategory(MeasurementCategory.Count).Contains(unit))
                        unitFamily = MeasurementCategory.Count;

                    var newIngredient = new Ingredient(Guid.NewGuid(), req.RequestedName, req.CategoryId ?? Guid.Empty, unitFamily);
                    ingredientsToAdd.Add(newIngredient);

                    req.ResolveAsApproved();
                    requestsToUpdate.Add(req);

                    var correspondingRecipeIngs = pendingRecipeIngredients.Where(ri => ri.IngredientRequestId == req.Id);
                    foreach (var ri in correspondingRecipeIngs)
                    {
                        ri.IngredientId = newIngredient.Id;
                    }
                }
            }

            if (ingredientsToAdd.Any())
            {
                await _ingredientRepository.AddRangeAsync(ingredientsToAdd, cancellationToken);
                await _cacheService.RemoveAsync("Pantry:AllIngredientsCatalog:v2");
            }
            if (requestsToUpdate.Any())
            {
                await _ingredientRequestRepository.UpdateRangeAsync(requestsToUpdate, cancellationToken);
            }
        }

        await _recipeRepository.UpdateAsync(recipe);

        var structuredIngredients = recipe.RecipeIngredients.Where(ri => ri.Ingredient != null).Select(ri =>
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

        var representativeCategory = recipe.RecipeIngredients.FirstOrDefault(ri => ri.Ingredient?.Category != null)?.Ingredient?.Category?.Name;

        await _aiRecommendationService.SyncRecipeAsync(
            recipe.Id,
            recipe.Title,
            structuredIngredients,
            representativeCategory,
            isPublished: true,
            cancellationToken: cancellationToken);

        return RecipeMessages.RecipeApprovedAndPublished;
    }
}
