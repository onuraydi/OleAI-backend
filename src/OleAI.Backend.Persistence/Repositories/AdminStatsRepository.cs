using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Admin.Queries.GetDashboardStats;
using OleAI.Backend.Persistence.Contexts;

namespace OleAI.Backend.Persistence.Repositories;

public class AdminStatsRepository : IAdminStatsRepository
{
    private readonly AppDbContext _dbContext;

    public AdminStatsRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<DailyStatPoint>> GetUserGrowthAsync(DateTime since, CancellationToken cancellationToken)
    {
        var data = await _dbContext.Users
            .Where(u => u.CreatedDate >= since)
            .GroupBy(u => u.CreatedDate.Date)
            .Select(g => new
            {
                Date = g.Key,
                Count = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToListAsync(cancellationToken);

        return data.Select(x => new DailyStatPoint
        {
            Date = x.Date.ToString("yyyy-MM-dd"),
            Count = x.Count
        }).ToList();
    }

    public async Task<List<DailyStatPoint>> GetRecipeGrowthAsync(DateTime since, CancellationToken cancellationToken)
    {
        var data = await _dbContext.Recipes
            .Where(r => r.CreatedDate >= since)
            .GroupBy(r => r.CreatedDate.Date)
            .Select(g => new
            {
                Date = g.Key,
                Count = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToListAsync(cancellationToken);

        return data.Select(x => new DailyStatPoint
        {
            Date = x.Date.ToString("yyyy-MM-dd"),
            Count = x.Count
        }).ToList();
    }

    public async Task<List<RecentActivityDto>> GetRecentActivitiesAsync(int count, CancellationToken cancellationToken)
    {
        var recentUsers = await _dbContext.Users
            .OrderByDescending(u => u.CreatedDate)
            .Take(count)
            .Select(u => new RecentActivityDto
            {
                Type = "new_user",
                Description = "Yeni kullanıcı kaydı: " + u.FirstName + " " + u.LastName,
                Timestamp = u.CreatedDate
            })
            .ToListAsync(cancellationToken);

        var recentRecipes = await _dbContext.Recipes
            .OrderByDescending(r => r.CreatedDate)
            .Take(count)
            .Select(r => new RecentActivityDto
            {
                Type = "new_recipe",
                Description = "Yeni tarif eklendi: " + r.Title,
                Timestamp = r.CreatedDate
            })
            .ToListAsync(cancellationToken);

        return recentUsers
            .Concat(recentRecipes)
            .OrderByDescending(a => a.Timestamp)
            .Take(count)
            .ToList();
    }
}
