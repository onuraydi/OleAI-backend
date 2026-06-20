using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Services.AiRecommendation;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using OleAI.Backend.Domain.Services;

namespace OleAI.Backend.Application.Features.Recipes.Queries.GetRecipesForAi
{
    public class GetRecipesForAiQuery : IRequest<List<RecipeForAiDto>>
    {
    }

    public class RecipeForAiDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<RecipeIngredientAiDto> Ingredients { get; set; } = new();
        public string? Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPublished { get; set; }
        public int Popularity { get; set; }
    }

    public class GetRecipesForAiQueryHandler : IRequestHandler<GetRecipesForAiQuery, List<RecipeForAiDto>>
    {
        private readonly IRecipeRepository _recipeRepository;

        public GetRecipesForAiQueryHandler(IRecipeRepository recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }

        public async Task<List<RecipeForAiDto>> Handle(GetRecipesForAiQuery request, CancellationToken cancellationToken)
        {
            var options = new QueryOptions<Recipe>
            {
                Predicate = r => r.DeletedDate == null && r.Status == RecipeStatus.Published,
                Include = q => q.Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient).ThenInclude(i => i.Aliases).Include(r => r.Interactions),
                Size = int.MaxValue,
                EnableTracking = false
            };

            var recipes = await _recipeRepository.GetListAsync(options, cancellationToken);

            return recipes.Items.Select(r => new RecipeForAiDto
            {
                Id = r.Id,
                Title = r.Title,
                Ingredients = r.RecipeIngredients.Where(ri => ri.Ingredient != null).Select(ri =>
                {
                    var baseUnit = ri.Ingredient!.UnitFamily switch
                    {
                        MeasurementCategory.Weight => MeasurementUnit.Gram,
                        MeasurementCategory.Volume => MeasurementUnit.Milliliter,
                        _ => MeasurementUnit.Piece
                    };
                    return new RecipeIngredientAiDto
                    {
                        Id = ri.Ingredient.Id,
                        Name = ri.Ingredient.Name,
                        Amount = MeasurementUnitHelper.ConvertAmount((double)ri.Amount, (MeasurementUnit)ri.UnitId, baseUnit),
                        Unit = baseUnit.ToString(),
                        Aliases = ri.Ingredient.Aliases.Select(a => a.AliasName).ToList()
                    };
                }).ToList(),
                Category = r.RecipeIngredients.FirstOrDefault()?.Ingredient?.Category?.Name,
                CreatedAt = r.CreatedDate,
                IsPublished = r.Status == RecipeStatus.Published,
                Popularity = r.Interactions.Count
            }).ToList();
        }
    }
}
