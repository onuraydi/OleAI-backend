using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Core.Repositories;
using Xunit;

namespace OleAI.Backend.Core.Tests.Repositories;

public class TestEntity : Entity<int>
{
    public string Name { get; set; } = string.Empty;
}

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public DbSet<TestEntity> TestEntities { get; set; }
}

public class TestRepository : EfRepositoryBase<TestEntity, int>
{
    public TestRepository(TestDbContext context) : base(context)
    {
    }
}

public class EfRepositoryBaseTests
{
    private readonly TestRepository _repository;
    private readonly TestDbContext _context;

    public EfRepositoryBaseTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        _repository = new TestRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddEntityAndSetId()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Test Add" };

        // Act
        var addedEntity = await _repository.AddAsync(entity);

        // Assert
        addedEntity.Should().NotBeNull();
        addedEntity.Id.Should().Be(1);
        _context.TestEntities.Count().Should().Be(1);
    }

    [Fact]
    public async Task GetListAsync_WithPagination_ShouldReturnPaginatedResult()
    {
        // Arrange
        var entities = new List<TestEntity>
        {
            new TestEntity { Id = 1, Name = "Item A" },
            new TestEntity { Id = 2, Name = "Item B" },
            new TestEntity { Id = 3, Name = "Item C" },
            new TestEntity { Id = 4, Name = "Item D" },
            new TestEntity { Id = 5, Name = "Item E" },
        };
        await _context.TestEntities.AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        // Act
        // Request Page 1 (index 0) with PageSize 2
        var result = await _repository.GetListAsync(index: 0, size: 2);

        // Assert
        result.Items.Count.Should().Be(2);
        result.TotalItems.Should().Be(5);
        result.TotalPages.Should().Be(3);
        result.PageNumber.Should().Be(1);
    }

    [Fact]
    public async Task DeleteAsync_WhenPermanentIsFalse_ShouldSoftDelete()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Soft Delete Me" };
        await _repository.AddAsync(entity);

        // Act
        await _repository.DeleteAsync(entity, permanent: false);

        // Assert
        var dbEntity = await _context.TestEntities.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == 1);
        dbEntity.Should().NotBeNull();
        dbEntity!.DeletedDate.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenPermanentIsTrue_ShouldHardDelete()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Hard Delete Me" };
        await _repository.AddAsync(entity);

        // Act
        await _repository.DeleteAsync(entity, permanent: true);

        // Assert
        var dbEntity = await _context.TestEntities.FirstOrDefaultAsync(e => e.Id == 1);
        dbEntity.Should().BeNull();
    }
}
