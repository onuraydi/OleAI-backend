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
                Include = q => q.Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient).Include(r => r.Interactions),
                Size = int.MaxValue,
                EnableTracking = false
            };

            var recipeList = await _recipeRepository.GetListAsync(options, cancellationToken);

            int successCount = 0;

            foreach (var recipe in recipeList.Items)
            {
                try
                {
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
                        popularity: recipe.Interactions.Count,
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
