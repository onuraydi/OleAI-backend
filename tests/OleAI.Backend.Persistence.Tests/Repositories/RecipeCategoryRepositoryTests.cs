using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Persistence.Contexts;
using OleAI.Backend.Persistence.Repositories;
using Xunit;

namespace OleAI.Backend.Persistence.Tests.Repositories
{
    public class RecipeCategoryRepositoryTests
    {
        private static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldPersistRecipeCategory()
        {
            using var context = CreateContext();
            var repository = new RecipeCategoryRepository(context);

            var category = new RecipeCategory(1, "Dessert");

            await repository.AddAsync(category);

            var saved = await context.RecipeCategories.FindAsync(1);
            saved.Should().NotBeNull();
            saved!.Name.Should().Be("Dessert");
        }
    }
}
