using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Pantry.Dtos.Responses;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Pantry.Queries.GetCategorizedPopularIngredients
{
    public class GetCategorizedPopularIngredientsQueryHandler : IRequestHandler<GetCategorizedPopularIngredientsQuery, List<CategorizedPopularIngredientsResponse>>
    {
        private readonly ICategoryRepository _categoryRepository;

        public GetCategorizedPopularIngredientsQueryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<CategorizedPopularIngredientsResponse>> Handle(GetCategorizedPopularIngredientsQuery request, CancellationToken cancellationToken)
        {
            var queryOptions = new QueryOptions<Category>
            {
                Include = c => c.Include(x => x.Ingredients).ThenInclude(i => i.UserIngredients),
                EnableTracking = false,
                Size = 100
            };

            var categoriesResult = await _categoryRepository.GetListAsync(queryOptions, cancellationToken);
            var categories = categoriesResult.Items.OrderBy(c => c.Name).ToList();

            var result = new List<CategorizedPopularIngredientsResponse>();

            foreach (var category in categories)
            {
                var topIngredients = category.Ingredients
                    .OrderByDescending(i => i.UserIngredients?.Count ?? 0)
                    .ThenBy(i => i.Name)
                    .Take(15)
                    .Select(i => new IngredientSearchResultResponse
                    {
                        MasterIngredientId = i.Id,
                        DisplayName = i.Name,
                        CategoryName = category.Name,
                        CategoryIconName = category.IconName,
                        CategoryId = category.Id,
                        UnitFamily = i.UnitFamily,
                        IsAlias = false
                    })
                    .ToList();

                if (topIngredients.Any())
                {
                    result.Add(new CategorizedPopularIngredientsResponse
                    {
                        CategoryId = category.Id,
                        CategoryName = category.Name,
                        IconName = category.IconName,
                        Ingredients = topIngredients
                    });
                }
            }

            return result;
        }
    }
}
