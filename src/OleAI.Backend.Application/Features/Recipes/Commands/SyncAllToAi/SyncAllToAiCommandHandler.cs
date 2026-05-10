using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Services.AiRecommendation;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Recipes.Commands.SyncAllToAi
{
    public class SyncAllToAiCommandHandler : IRequestHandler<SyncAllToAiCommand, string>
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IAiRecommendationService _aiRecommendationService;

        public SyncAllToAiCommandHandler(IRecipeRepository recipeRepository, IAiRecommendationService aiRecommendationService)
        {
            _recipeRepository = recipeRepository;
            _aiRecommendationService = aiRecommendationService;
        }

        public async Task<string> Handle(SyncAllToAiCommand request, CancellationToken cancellationToken)
        {
            // Tüm yayınlanmış tarifleri malzemeleriyle birlikte çek
            var options = new QueryOptions<Recipe>
            {
                Predicate = r => r.Status == RecipeStatus.Published,
                Include = q => q.Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient),
                Size = int.MaxValue,
                EnableTracking = false
            };

            var recipeList = await _recipeRepository.GetListAsync(options, cancellationToken);

            int successCount = 0;

            foreach (var recipe in recipeList.Items)
            {
                try
                {
                    var ingredientText = string.Join(" ", recipe.RecipeIngredients.Select(ri => ri.Ingredient.Name));
                    var representativeCategory = recipe.RecipeIngredients.FirstOrDefault()?.Ingredient?.Category?.Name;

                    await _aiRecommendationService.SyncRecipeAsync(
                        recipe.Id,
                        recipe.Title,
                        ingredientText,
                        representativeCategory,
                        cancellationToken: cancellationToken);

                    successCount++;
                }
                catch
                {
                    // Log error and continue with next recipe
                }
            }

            return $"{successCount} recipes successfully synced with AI engine.";
        }
    }
}
