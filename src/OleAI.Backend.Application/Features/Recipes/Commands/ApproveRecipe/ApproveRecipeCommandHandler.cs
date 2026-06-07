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
            include: q => q.Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient),
            cancellationToken: cancellationToken);

        _recipeBusinessRules.CheckIfRecipeExists(recipe);

        recipe!.ApproveByAdmin();

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

        var ingredientText = string.Join(" ", recipe.RecipeIngredients.Where(ri => ri.Ingredient != null).Select(ri => ri.Ingredient!.Name));
        var representativeCategory = recipe.RecipeIngredients.FirstOrDefault(ri => ri.Ingredient?.Category != null)?.Ingredient?.Category?.Name;

        await _aiRecommendationService.SyncRecipeAsync(
            recipe.Id,
            recipe.Title,
            ingredientText,
            representativeCategory,
            cancellationToken: cancellationToken);

        return RecipeMessages.RecipeApprovedAndPublished;
    }
}
