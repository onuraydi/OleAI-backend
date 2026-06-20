using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Recipes.Queries.GetAllAdminRecipes
{
    public class GetAllAdminRecipesQueryHandler : IRequestHandler<GetAllAdminRecipesQuery, Paginate<AdminRecipeListItemDto>>
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IUserRepository _userRepository;

        public GetAllAdminRecipesQueryHandler(IRecipeRepository recipeRepository, IUserRepository userRepository)
        {
            _recipeRepository = recipeRepository;
            _userRepository = userRepository;
        }

        public async Task<Paginate<AdminRecipeListItemDto>> Handle(GetAllAdminRecipesQuery request, CancellationToken cancellationToken)
        {
            var query = new QueryOptions<Recipe>();
            query.EnableTracking = false;
            // The frontend usually sends 1-indexed pages, but backend uses 0-indexed query in some places.
            // Let's assume request.Page is 0-indexed for repository.
            query.Index = request.Page;
            query.Size = request.Size;

            // Sıralama
            query.OrderBy = q => q.OrderByDescending(r => r.CreatedDate);

            // Filtreler
            var hasStatus = request.Status.HasValue;
            var statusVal = request.Status ?? RecipeStatus.Published;
            var hasVis = request.Visibility.HasValue;
            var visVal = request.Visibility ?? RecipeVisibility.Public;
            var hasSearch = !string.IsNullOrWhiteSpace(request.SearchTerm);
            var search = request.SearchTerm?.ToLower() ?? "";

            query.Predicate = r =>
                r.DeletedDate == null &&
                (!hasStatus || r.Status == statusVal) &&
                (!hasVis || r.Visibility == visVal) &&
                (!hasSearch || r.Title.ToLower().Contains(search));

            var recipes = await _recipeRepository.GetListAsync(query, cancellationToken: cancellationToken);

            var resultPaginate = new Paginate<AdminRecipeListItemDto>
            {
                PageNumber = recipes.PageNumber,
                PageSize = recipes.PageSize,
                TotalItems = recipes.TotalItems,
                TotalPages = recipes.TotalPages,
                Items = new List<AdminRecipeListItemDto>()
            };

            // Yazarların bilgilerini çekelim
            var userIds = recipes.Items.Select(r => r.UserId).Distinct().ToList();
            var users = await _userRepository.GetListAsync(
                new QueryOptions<User>
                {
                    Predicate = u => userIds.Contains(u.Id),
                    EnableTracking = false
                },
                cancellationToken: cancellationToken);

            var userDict = users.Items.ToDictionary(u => u.Id, u => u);

            foreach (var recipe in recipes.Items)
            {
                var userName = "Bilinmeyen Kullanıcı";
                if (userDict.TryGetValue(recipe.UserId, out var user))
                {
                    userName = $"{user.FirstName} {user.LastName}";
                }

                resultPaginate.Items.Add(new AdminRecipeListItemDto
                {
                    Id = recipe.Id,
                    UserId = recipe.UserId,
                    UserName = userName,
                    Title = recipe.Title,
                    Status = recipe.Status.ToString(),
                    Visibility = recipe.Visibility.ToString(),
                    CreatedDate = recipe.CreatedDate
                });
            }

            return resultPaginate;
        }
    }
}
