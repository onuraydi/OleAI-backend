using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.AiRecommendation;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.Recipes.Commands.ApproveRecipe;

public class ApproveRecipeCommandHandler : IRequestHandler<ApproveRecipeCommand, string>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly RecipeBusinessRules _recipeBusinessRules;
    private readonly IAiRecommendationService _aiRecommendationService;

    public ApproveRecipeCommandHandler(
        IRecipeRepository recipeRepository,
        RecipeBusinessRules recipeBusinessRules,
        IAiRecommendationService aiRecommendationService)
    {
        _recipeRepository = recipeRepository;
        _recipeBusinessRules = recipeBusinessRules;
        _aiRecommendationService = aiRecommendationService;
    }

    public async Task<string> Handle(ApproveRecipeCommand request, CancellationToken cancellationToken)
    {
        var recipe = await _recipeRepository.GetAsync(
            predicate: r => r.Id == request.Id,
            include: q => q.Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient),
            cancellationToken: cancellationToken);

        _recipeBusinessRules.CheckIfRecipeExists(recipe);

        recipe!.ApproveByAdmin();

        await _recipeRepository.UpdateAsync(recipe);

        // AI Servisini bilgilendir
        var ingredientText = string.Join(" ", recipe.RecipeIngredients.Select(ri => ri.Ingredient.Name));
        var representativeCategory = recipe.RecipeIngredients.FirstOrDefault()?.Ingredient?.Category?.Name;

        await _aiRecommendationService.SyncRecipeAsync(
            recipe.Id,
            recipe.Title,
            ingredientText,
            representativeCategory,
            cancellationToken: cancellationToken);

        return RecipeMessages.RecipeApprovedAndPublished;
    }
}
