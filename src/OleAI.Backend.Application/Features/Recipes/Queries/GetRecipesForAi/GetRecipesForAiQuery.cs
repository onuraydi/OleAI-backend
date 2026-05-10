using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Recipes.Queries.GetRecipesForAi
{
    public class GetRecipesForAiQuery : IRequest<List<RecipeForAiDto>>
    {
    }

    public class RecipeForAiDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Ingredients { get; set; } = string.Empty;
        public string? Category { get; set; }
        public DateTime CreatedAt { get; set; }
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
                Predicate = r => r.Status == RecipeStatus.Published,
                Include = q => q.Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient),
                Size = int.MaxValue,
                EnableTracking = false
            };

            var recipes = await _recipeRepository.GetListAsync(options, cancellationToken);

            return recipes.Items.Select(r => new RecipeForAiDto
            {
                Id = r.Id,
                Title = r.Title,
                Ingredients = string.Join(" ", r.RecipeIngredients.Select(ri => ri.Ingredient.Name)),
                Category = r.RecipeIngredients.FirstOrDefault()?.Ingredient?.Category?.Name,
                CreatedAt = r.CreatedDate
            }).ToList();
        }
    }
}
