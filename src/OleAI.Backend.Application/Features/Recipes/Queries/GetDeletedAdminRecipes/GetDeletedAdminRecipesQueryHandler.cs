using AutoMapper;
using MediatR;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Recipes.Queries.GetDeletedAdminRecipes;

public class GetDeletedAdminRecipesQueryHandler : IRequestHandler<GetDeletedAdminRecipesQuery, Paginate<AdminRecipeListItemDto>>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUserRepository _userRepository;

    public GetDeletedAdminRecipesQueryHandler(IRecipeRepository recipeRepository, IUserRepository userRepository)
    {
        _recipeRepository = recipeRepository;
        _userRepository = userRepository;
    }

    public async Task<Paginate<AdminRecipeListItemDto>> Handle(GetDeletedAdminRecipesQuery request, CancellationToken cancellationToken)
    {
        var query = new QueryOptions<Recipe>();
        query.EnableTracking = false;
        query.Index = request.Page;
        query.Size = request.Size;
        query.WithDeleted = true; // VERY IMPORTANT: Includes soft deleted items

        query.OrderBy = q => q.OrderByDescending(r => r.DeletedDate);

        var hasSearch = !string.IsNullOrWhiteSpace(request.SearchTerm);
        var search = request.SearchTerm?.ToLower() ?? "";

        // Sadece silinmişleri getir (DeletedDate != null)
        query.Predicate = r =>
            r.DeletedDate != null &&
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
