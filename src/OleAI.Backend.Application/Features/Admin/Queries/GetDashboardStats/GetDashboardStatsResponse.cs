namespace OleAI.Backend.Application.Features.Admin.Queries.GetDashboardStats;

public class GetDashboardStatsResponse
{
    // Summary Cards
    public int TotalUsers { get; set; }
    public int TotalRecipes { get; set; }
    public int TotalComments { get; set; }
    public int TotalIngredients { get; set; }

    // Recipe Status Distribution
    public int RecipesPendingLlm { get; set; }
    public int RecipesPendingAdmin { get; set; }
    public int RecipesPublished { get; set; }
    public int RecipesRejected { get; set; }

    // Interaction Metrics
    public int TotalLikes { get; set; }
    public int TotalSaves { get; set; }
    public int TotalDislikes { get; set; }

    // Ingredient Requests
    public int PendingIngredientRequests { get; set; }
    public int ResolvedIngredientRequests { get; set; }

    // 30-Day Growth Data
    public List<DailyStatPoint> UserGrowth { get; set; } = new();
    public List<DailyStatPoint> RecipeGrowth { get; set; } = new();

    // Difficulty Distribution
    public int EasyRecipes { get; set; }
    public int MediumRecipes { get; set; }
    public int HardRecipes { get; set; }

    // Recent Activities
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
}

public class DailyStatPoint
{
    public string Date { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class RecentActivityDto
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
