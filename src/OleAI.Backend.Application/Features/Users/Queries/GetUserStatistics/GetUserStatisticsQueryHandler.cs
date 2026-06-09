using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.Users.Queries.GetUserStatistics;

public class GetUserStatisticsQueryHandler : IRequestHandler<GetUserStatisticsQuery, GetUserStatisticsResponse>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IRecipeInteractionRepository _recipeInteractionRepository;
    private readonly IUserIngredientRepository _userIngredientRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetUserStatisticsQueryHandler(
        IRecipeRepository recipeRepository,
        IRecipeInteractionRepository recipeInteractionRepository,
        IUserIngredientRepository userIngredientRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _recipeRepository = recipeRepository;
        _recipeInteractionRepository = recipeInteractionRepository;
        _userIngredientRepository = userIngredientRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<GetUserStatisticsResponse> Handle(GetUserStatisticsQuery request, CancellationToken cancellationToken)
    {
        var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var totalRecipesCreated = await _recipeRepository.GetCountAsync(
            predicate: r => r.UserId == userId,
            cancellationToken: cancellationToken);

        var totalRecipesSaved = await _recipeInteractionRepository.GetCountAsync(
            predicate: ri => ri.UserId == userId && ri.IsSaved,
            cancellationToken: cancellationToken);

        var totalRecipesLiked = await _recipeInteractionRepository.GetCountAsync(
            predicate: ri => ri.UserId == userId && ri.IsLiked,
            cancellationToken: cancellationToken);

        var totalPantryItems = await _userIngredientRepository.GetCountAsync(
            predicate: ui => ui.UserId == userId,
            cancellationToken: cancellationToken);

        return new GetUserStatisticsResponse
        {
            TotalRecipesCreated = totalRecipesCreated,
            TotalRecipesSaved = totalRecipesSaved,
            TotalRecipesLiked = totalRecipesLiked,
            TotalPantryItems = totalPantryItems
        };
    }
}
