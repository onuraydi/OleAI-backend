using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Core.Repositories.Pagination;
using Xunit;

namespace OleAI.Backend.Core.Tests.Repositories.Pagination;

public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public DbSet<TestEntity> TestEntities { get; set; }
}

public class IQueryablePaginateExtensionsTests
{
    private readonly TestDbContext _context;

    public IQueryablePaginateExtensionsTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new TestDbContext(options);
    }

    [Fact]
    public async Task ToPaginateAsync_WithData_ShouldReturnCorrectPaginationMetadata()
    {
        // Arrange
        var entities = Enumerable.Range(1, 10).Select(i => new TestEntity { Id = i, Name = $"Entity {i}" });
        await _context.TestEntities.AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        // Index 0, Size 3 -> Page 1, Items 1-3
        // Act
        var result = await _context.TestEntities.ToPaginateAsync(index: 0, size: 3);

        // Assert
        result.Items.Count.Should().Be(3);
        result.TotalItems.Should().Be(10);
        result.TotalPages.Should().Be(4); // 10/3 = 3.33 -> 4
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(3);
        result.Items.First().Id.Should().Be(1);
    }

    [Fact]
    public async Task ToPaginateAsync_WithSecondPage_ShouldReturnCorrectItems()
    {
        // Arrange
        var entities = Enumerable.Range(1, 5).Select(i => new TestEntity { Id = i, Name = $"Entity {i}" });
        await _context.TestEntities.AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        // Index 1, Size 2 -> Page 2, Items 3-4
        // Act
        var result = await _context.TestEntities.ToPaginateAsync(index: 1, size: 2);

        // Assert
        result.Items.Count.Should().Be(2);
        result.Items.First().Id.Should().Be(3);
        result.PageNumber.Should().Be(2);
    }

    [Fact]
    public async Task ToPaginateAsync_WithEmptySource_ShouldReturnEmptyResult()
    {
        // Act
        var result = await _context.TestEntities.ToPaginateAsync(index: 0, size: 10);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalItems.Should().Be(0);
        result.TotalPages.Should().Be(0);
        result.PageNumber.Should().Be(1);
    }

    [Fact]
    public async Task ToPaginateAsync_WithCustomIndex_ShouldHandleCorrectPageNumber()
    {
        // Arrange
        await _context.TestEntities.AddAsync(new TestEntity { Id = 1 });
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.TestEntities.ToPaginateAsync(index: 5, size: 10);

        // Assert
        result.PageNumber.Should().Be(6); // index + 1
        result.Items.Should().BeEmpty();
    }
}
