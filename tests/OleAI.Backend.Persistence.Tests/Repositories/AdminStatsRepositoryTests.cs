using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Core.Repositories;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using OleAI.Backend.Persistence.Contexts;
using OleAI.Backend.Persistence.Repositories;
using Xunit;

namespace OleAI.Backend.Persistence.Tests.Repositories
{
    public class AdminStatsRepositoryTests
    {
        private static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetUserGrowthAsync_ShouldReturnDailyStatPoints()
        {
            using var context = CreateContext();
            var repository = new AdminStatsRepository(context);

            var today = DateTime.UtcNow.Date;

            // Add users
            context.Users.Add(new User { Id = Guid.NewGuid(), FirstName = "U1", LastName = "L1", Email = "u1@test.com", CreatedDate = today });
            context.Users.Add(new User { Id = Guid.NewGuid(), FirstName = "U2", LastName = "L2", Email = "u2@test.com", CreatedDate = today });
            context.Users.Add(new User { Id = Guid.NewGuid(), FirstName = "U3", LastName = "L3", Email = "u3@test.com", CreatedDate = today.AddDays(-1) });
            await context.SaveChangesAsync();

            var result = await repository.GetUserGrowthAsync(today.AddDays(-2), CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);

            var todayStat = result.FirstOrDefault(x => x.Date == today.ToString("yyyy-MM-dd"));
            todayStat.Should().NotBeNull();
            todayStat!.Count.Should().Be(2);

            var yesterdayStat = result.FirstOrDefault(x => x.Date == today.AddDays(-1).ToString("yyyy-MM-dd"));
            yesterdayStat.Should().NotBeNull();
            yesterdayStat!.Count.Should().Be(1);
        }

        [Fact]
        public async Task GetRecipeGrowthAsync_ShouldReturnDailyStatPoints()
        {
            using var context = CreateContext();
            var repository = new AdminStatsRepository(context);

            var today = DateTime.UtcNow.Date;

            // Add recipes
            var r1 = Recipe.Create(Guid.NewGuid(), "R1", "Desc1", RecipeDifficulty.Easy, 2, 10, 20, RecipeVisibility.Public);
            // Override CreatedDate using reflection or just add it since EF can override it if we save changes. Wait, we can't easily override CreatedDate.
            // Actually, we can use reflection if needed, but since EF Core saves entities, we might need a workaround for CreatedDate or just set it via reflection for the test.
            // Let's use reflection.
            typeof(Entity<Guid>).GetProperty("CreatedDate")?.SetValue(r1, today);
            context.Recipes.Add(r1);

            var r2 = Recipe.Create(Guid.NewGuid(), "R2", "Desc2", RecipeDifficulty.Easy, 2, 10, 20, RecipeVisibility.Public);
            typeof(Entity<Guid>).GetProperty("CreatedDate")?.SetValue(r2, today.AddDays(-2));
            context.Recipes.Add(r2);
            await context.SaveChangesAsync();

            var result = await repository.GetRecipeGrowthAsync(today.AddDays(-3), CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);

            var todayStat = result.FirstOrDefault(x => x.Date == today.ToString("yyyy-MM-dd"));
            todayStat.Should().NotBeNull();
            todayStat!.Count.Should().Be(1);

            var pastStat = result.FirstOrDefault(x => x.Date == today.AddDays(-2).ToString("yyyy-MM-dd"));
            pastStat.Should().NotBeNull();
            pastStat!.Count.Should().Be(1);
        }

        [Fact]
        public async Task GetRecentActivitiesAsync_ShouldReturnCombinedActivities()
        {
            using var context = CreateContext();
            var repository = new AdminStatsRepository(context);

            var now = DateTime.UtcNow;

            context.Users.Add(new User { Id = Guid.NewGuid(), FirstName = "U1", LastName = "L1", Email = "u1@test.com", CreatedDate = now.AddMinutes(-5) });
            var r1 = Recipe.Create(Guid.NewGuid(), "R1", "Desc", RecipeDifficulty.Easy, 2, 10, 20, RecipeVisibility.Public);
            typeof(Entity<Guid>).GetProperty("CreatedDate")?.SetValue(r1, now.AddMinutes(-10));
            context.Recipes.Add(r1);
            context.Users.Add(new User { Id = Guid.NewGuid(), FirstName = "U2", LastName = "L2", Email = "u2@test.com", CreatedDate = now.AddMinutes(-1) });

            await context.SaveChangesAsync();

            var result = await repository.GetRecentActivitiesAsync(2, CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);

            // Expected order: descending by timestamp (U2 first, then U1)
            result[0].Type.Should().Be("new_user");
            result[0].Description.Should().Contain("U2");
            result[1].Type.Should().Be("new_user");
            result[1].Description.Should().Contain("U1");
        }
    }
}
