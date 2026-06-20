namespace OleAI.Backend.Application.Features.Admin.Queries.GetDashboardStats;

public interface IAdminStatsRepository
{
    Task<List<DailyStatPoint>> GetUserGrowthAsync(DateTime since, CancellationToken cancellationToken);
    Task<List<DailyStatPoint>> GetRecipeGrowthAsync(DateTime since, CancellationToken cancellationToken);
    Task<List<RecentActivityDto>> GetRecentActivitiesAsync(int count, CancellationToken cancellationToken);
}
