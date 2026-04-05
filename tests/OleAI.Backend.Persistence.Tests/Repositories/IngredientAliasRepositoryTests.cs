using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Persistence.Contexts;
using OleAI.Backend.Persistence.Repositories;

namespace OleAI.Backend.Persistence.Tests.Repositories;

public class IngredientAliasRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistAlias()
    {
        using var context = CreateContext();
        var repository = new IngredientAliasRepository(context);

        var ingredient = new Ingredient(Guid.NewGuid(), "Domates", Guid.NewGuid(), "adet");
        await context.Ingredients.AddAsync(ingredient);
        await context.SaveChangesAsync();

        var alias = new IngredientAlias(Guid.NewGuid(), ingredient.Id, "Domat");
        await repository.AddAsync(alias);

        var saved = await context.IngredientAliases.FindAsync(alias.Id);
        saved.Should().NotBeNull();
        saved!.AliasName.Should().Be("Domat");
    }

    [Fact]
    public async Task GetAsync_ShouldReturnAliasByName()
    {
        using var context = CreateContext();
        var repository = new IngredientAliasRepository(context);

        var ingredient = new Ingredient(Guid.NewGuid(), "Biber", Guid.NewGuid(), "gram");
        var alias = new IngredientAlias(Guid.NewGuid(), ingredient.Id, "Pul biber");

        await context.Ingredients.AddAsync(ingredient);
        await context.IngredientAliases.AddAsync(alias);
        await context.SaveChangesAsync();

        var fetched = await repository.GetAsync(a => a.AliasName == "Pul biber");

        fetched.Should().NotBeNull();
        fetched!.Id.Should().Be(alias.Id);
    }
}
