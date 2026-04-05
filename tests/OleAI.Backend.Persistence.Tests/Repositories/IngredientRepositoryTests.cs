using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Persistence.Contexts;
using OleAI.Backend.Persistence.Repositories;

namespace OleAI.Backend.Persistence.Tests.Repositories
{
    public class IngredientRepositoryTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddAsync_Should_Successfully_Save_Ingredient_With_Aliases()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new IngredientRepository(context);

            var categoryId = Guid.NewGuid();
            var ingredientId = Guid.NewGuid();
            var ingredient = new Ingredient(ingredientId, "Domates", categoryId, "Adet");

            ingredient.Aliases.Add(new IngredientAlias(Guid.NewGuid(), ingredientId, "Domat"));
            ingredient.Aliases.Add(new IngredientAlias(Guid.NewGuid(), ingredientId, "Salkım Domates"));

            // Act
            await repository.AddAsync(ingredient);

            // Assert
            var savedIngredient = await context.Ingredients
                .Include(i => i.Aliases)
                .FirstOrDefaultAsync(i => i.Id == ingredientId);

            savedIngredient.Should().NotBeNull();
            savedIngredient!.Name.Should().Be("Domates");
            savedIngredient.Aliases.Should().HaveCount(2);
            savedIngredient.Aliases.Any(a => a.AliasName == "Domat").Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Ingredient()
        {
            // Arrange
            using var context = GetInMemoryDbContext();

            var repository = new IngredientRepository(context);

            var ingredient = new Ingredient(Guid.NewGuid(), "Şeker", Guid.NewGuid(), "Gram");
            await context.Ingredients.AddAsync(ingredient);
            await context.SaveChangesAsync();

            // Act
            await repository.DeleteAsync(ingredient, permanent: true);

            // Assert
            var deletedIngredient = await context.Ingredients.FindAsync(ingredient.Id);
            deletedIngredient.Should().BeNull("Çünkü veritabanından silinmiş olmalı.");
        }
    }
}
