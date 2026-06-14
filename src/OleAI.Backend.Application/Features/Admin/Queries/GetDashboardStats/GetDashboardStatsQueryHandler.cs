using MediatR;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Admin.Queries.GetDashboardStats;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, GetDashboardStatsResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IRecipeInteractionRepository _interactionRepository;
    private readonly IRecipeCommentRepository _commentRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IIngredientRequestRepository _ingredientRequestRepository;
    private readonly IAdminStatsRepository _adminStatsRepository;

    public GetDashboardStatsQueryHandler(
        IUserRepository userRepository,
        IRecipeRepository recipeRepository,
        IRecipeInteractionRepository interactionRepository,
        IRecipeCommentRepository commentRepository,
        IIngredientRepository ingredientRepository,
        IIngredientRequestRepository ingredientRequestRepository,
        IAdminStatsRepository adminStatsRepository)
    {
        _userRepository = userRepository;
        _recipeRepository = recipeRepository;
        _interactionRepository = interactionRepository;
        _commentRepository = commentRepository;
        _ingredientRepository = ingredientRepository;
        _ingredientRequestRepository = ingredientRequestRepository;
        _adminStatsRepository = adminStatsRepository;
    }

    public async Task<GetDashboardStatsResponse> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

        // Summary Counts
        var totalUsers = await _userRepository.GetCountAsync(cancellationToken: cancellationToken);
        var totalRecipes = await _recipeRepository.GetCountAsync(cancellationToken: cancellationToken);
        var totalComments = await _commentRepository.GetCountAsync(cancellationToken: cancellationToken);
        var totalIngredients = await _ingredientRepository.GetCountAsync(cancellationToken: cancellationToken);

        // Recipe Status Distribution
        var pendingLlm = await _recipeRepository.GetCountAsync(r => r.Status == RecipeStatus.PendingLlmApproval, cancellationToken: cancellationToken);
        var pendingAdmin = await _recipeRepository.GetCountAsync(r => r.Status == RecipeStatus.PendingAdminApproval, cancellationToken: cancellationToken);
        var published = await _recipeRepository.GetCountAsync(r => r.Status == RecipeStatus.Published, cancellationToken: cancellationToken);
        var rejected = await _recipeRepository.GetCountAsync(r => r.Status == RecipeStatus.Rejected, cancellationToken: cancellationToken);

        // Interaction Metrics
        var totalLikes = await _interactionRepository.GetCountAsync(i => i.IsLiked, cancellationToken: cancellationToken);
        var totalSaves = await _interactionRepository.GetCountAsync(i => i.IsSaved, cancellationToken: cancellationToken);
        var totalDislikes = await _interactionRepository.GetCountAsync(i => i.IsDisliked, cancellationToken: cancellationToken);

        // Ingredient Requests
        var pendingRequests = await _ingredientRequestRepository.GetCountAsync(
            r => r.Status == IngredientRequestStatus.Pending, cancellationToken: cancellationToken);
        var resolvedRequests = await _ingredientRequestRepository.GetCountAsync(
            r => r.Status != IngredientRequestStatus.Pending, cancellationToken: cancellationToken);

        // Difficulty Distribution
        var easyRecipes = await _recipeRepository.GetCountAsync(r => r.Difficulty == RecipeDifficulty.Easy, cancellationToken: cancellationToken);
        var mediumRecipes = await _recipeRepository.GetCountAsync(r => r.Difficulty == RecipeDifficulty.Medium, cancellationToken: cancellationToken);
        var hardRecipes = await _recipeRepository.GetCountAsync(r => r.Difficulty == RecipeDifficulty.Hard, cancellationToken: cancellationToken);

        // Growth Data (30 days)
        var userGrowth = await _adminStatsRepository.GetUserGrowthAsync(thirtyDaysAgo, cancellationToken);
        var recipeGrowth = await _adminStatsRepository.GetRecipeGrowthAsync(thirtyDaysAgo, cancellationToken);

        // Recent Activities
        var recentActivities = await _adminStatsRepository.GetRecentActivitiesAsync(10, cancellationToken);

        return new GetDashboardStatsResponse
        {
            TotalUsers = totalUsers,
            TotalRecipes = totalRecipes,
            TotalComments = totalComments,
            TotalIngredients = totalIngredients,
            RecipesPendingLlm = pendingLlm,
            RecipesPendingAdmin = pendingAdmin,
            RecipesPublished = published,
            RecipesRejected = rejected,
            TotalLikes = totalLikes,
            TotalSaves = totalSaves,
            TotalDislikes = totalDislikes,
            PendingIngredientRequests = pendingRequests,
            ResolvedIngredientRequests = resolvedRequests,
            UserGrowth = userGrowth,
            RecipeGrowth = recipeGrowth,
            EasyRecipes = easyRecipes,
            MediumRecipes = mediumRecipes,
            HardRecipes = hardRecipes,
            RecentActivities = recentActivities
        };
    }
}
