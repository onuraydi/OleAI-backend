using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Persistence.Contexts;
using OleAI.Backend.Persistence.Repositories;

namespace OleAI.Backend.Persistence.Tests.Repositories;

public class CategoryRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistCategory()
    {
        using var context = CreateContext();
        var repository = new CategoryRepository(context);

        var category = new Category(Guid.NewGuid(), "Baharat");

        await repository.AddAsync(category);

        var saved = await context.Categories.FindAsync(category.Id);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("Baharat");
    }

    [Fact]
    public async Task DeleteAsync_Permanent_ShouldRemoveCategory()
    {
        using var context = CreateContext();
        var repository = new CategoryRepository(context);

        var category = new Category(Guid.NewGuid(), "Süt Ürünleri");
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        await repository.DeleteAsync(category, permanent: true);

        (await context.Categories.FindAsync(category.Id)).Should().BeNull();
    }
}
