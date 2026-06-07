using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Pantry.Dtos.Responses;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Pantry.Queries.SearchIngredient
{
    public class SearchIngredientQueryHandler : IRequestHandler<SearchIngredientQuery, List<IngredientSearchResultResponse>>
    {
        private readonly IIngredientRepository _igredientRepository;
        private readonly ICacheService _cacheService;
        private const string CacheKey = "Pantry:AllIngredientsCatalog:v2";
        private const int MinimumSearchLength = 2;

        public SearchIngredientQueryHandler(IIngredientRepository igredientRepository, ICacheService cacheService)
        {
            _igredientRepository = igredientRepository;
            _cacheService = cacheService;
        }

        public async Task<List<IngredientSearchResultResponse>> Handle(SearchIngredientQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Keyword) || request.Keyword.Length < MinimumSearchLength)
                return new List<IngredientSearchResultResponse>();

            var cachedCatalog = await _cacheService.GetAsync<List<IngredientSearchResultResponse>>(CacheKey);

            if (cachedCatalog == null)
            {
                cachedCatalog = await LoadCatalogFromDatabaseAsync(cancellationToken);
                // 6 saat veri tabanına gitmez çekilen katalog kullanılır
                await _cacheService.SetAsync(CacheKey, cachedCatalog, TimeSpan.FromHours(6));
            }

            // Bellek içi filtreleme
            var searchKeyword = request.Keyword.ToLowerInvariant();

            var results = cachedCatalog
                .Where(x => x.DisplayName.ToLowerInvariant().Contains(searchKeyword))
                // Sıralama Algoritması: Aranan kelimeyle BAŞLAYANLAR (StartsWith) her zaman üstte çıksın, sonra alfabeye göre dizilsin.
                .OrderBy(x => x.DisplayName.StartsWith(searchKeyword, StringComparison.InvariantCultureIgnoreCase) ? 0 : 1)
                .ThenBy(x => x.DisplayName)
                .Take(20)  // Sadece ilk 20 sonuç
                .ToList();

            return results;
        }

        private async Task<List<IngredientSearchResultResponse>> LoadCatalogFromDatabaseAsync(CancellationToken cancellationToken)
        {
            QueryOptions<Ingredient> query = new QueryOptions<Ingredient>();
            query.Include = q => q.Include(i => i.Aliases).Include(i => i.Category);
            query.EnableTracking = false;
            query.Size = 25000; // tüm veriler için yeterince büyük bir değer

            var ingredients = await _igredientRepository.GetListAsync(query, cancellationToken);

            var catalog = new List<IngredientSearchResultResponse>();

            foreach (var ingredient in ingredients.Items)
            {
                // Arama havuzuna Master kelimeyi ekle
                catalog.Add(new IngredientSearchResultResponse
                {
                    MasterIngredientId = ingredient.Id,
                    DisplayName = ingredient.Name,
                    IsAlias = false,
                    UnitFamily = ingredient.UnitFamily,
                    CategoryId = ingredient.CategoryId,
                    CategoryName = ingredient.Category?.Name ?? "Diğer"
                });

                // Arama havuzuna Eşanlamlı (Alias) kelimeleri ekle
                foreach (var alias in ingredient.Aliases)
                {
                    catalog.Add(new IngredientSearchResultResponse
                    {
                        MasterIngredientId = ingredient.Id, // Kritik: Alias da seçilse Master ID dönüyor!
                        DisplayName = alias.AliasName,
                        IsAlias = true,
                        UnitFamily = ingredient.UnitFamily,
                        CategoryId = ingredient.CategoryId,
                        CategoryName = ingredient.Category?.Name ?? "Diğer"
                    });
                }
            }
            return catalog;
        }
    }
}
