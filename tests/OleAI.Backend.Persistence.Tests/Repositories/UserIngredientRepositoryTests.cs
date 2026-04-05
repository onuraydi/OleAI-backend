using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Persistence.Contexts;
using OleAI.Backend.Persistence.Repositories;

namespace OleAI.Backend.Persistence.Tests.Repositories
{
    public class UserIngredientRepositoryTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetListAsync_Should_Return_Only_Specific_Users_Ingredients()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new UserIngredientRepository(context);

            var targetUserId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            await context.UserIngredients.AddAsync(UserIngredient.CreateStandard(Guid.NewGuid(), targetUserId, Guid.NewGuid(), 500));
            await context.UserIngredients.AddAsync(UserIngredient.CreatePending(Guid.NewGuid(), targetUserId, Guid.NewGuid(), 2));

            await context.UserIngredients.AddAsync(UserIngredient.CreateStandard(Guid.NewGuid(), otherUserId, Guid.NewGuid(), 100));
            await context.SaveChangesAsync();

            // Act
            var options = new QueryOptions<UserIngredient>();
            options.Predicate = ui => ui.UserId == targetUserId;
            options.EnableTracking = false;

            var result = await repository.GetListAsync(options);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2, "Hedef kullanıcının sadece 2 ürünü var.");
            result.Items.All(i => i.UserId == targetUserId).Should().BeTrue();
        }
    }
}
