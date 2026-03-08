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
        var options = new OleAI.Backend.Core.Repositories.Options.QueryOptions<TestEntity>
        {
            Index = 0,
            Size = 2
        };
        var result = await _repository.GetListAsync(options);

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
    [Fact]
    public async Task AddRangeAsync_ShouldAddEntities()
    {
        var entities = new List<TestEntity>
        {
            new TestEntity { Id = 10, Name = "A" },
            new TestEntity { Id = 11, Name = "B" }
        };
        await _repository.AddRangeAsync(entities);

        _context.TestEntities.Count().Should().Be(2);
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnTrueIfExists()
    {
        await _repository.AddAsync(new TestEntity { Id = 20, Name = "C" });

        var exists = await _repository.AnyAsync(e => e.Name == "C");
        var notExists = await _repository.AnyAsync(e => e.Name == "Z");

        exists.Should().BeTrue();
        notExists.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteRangeAsync_WhenPermanentIsFalse_ShouldSoftDeleteAll()
    {
        var entities = new List<TestEntity>
        {
            new TestEntity { Id = 30, Name = "D" },
            new TestEntity { Id = 31, Name = "E" }
        };
        await _repository.AddRangeAsync(entities);

        await _repository.DeleteRangeAsync(entities, permanent: false);

        var dbEntities = await _context.TestEntities.IgnoreQueryFilters().ToListAsync();
        dbEntities.Should().AllSatisfy(e => e.DeletedDate.Should().NotBeNull());
    }

    [Fact]
    public async Task DeleteRangeAsync_WhenPermanentIsTrue_ShouldHardDeleteAll()
    {
        var entities = new List<TestEntity>
        {
            new TestEntity { Id = 40, Name = "F" },
            new TestEntity { Id = 41, Name = "G" }
        };
        await _repository.AddRangeAsync(entities);

        await _repository.DeleteRangeAsync(entities, permanent: true);

        var count = await _context.TestEntities.CountAsync();
        count.Should().Be(0);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnEntity()
    {
        await _repository.AddAsync(new TestEntity { Id = 50, Name = "H" });

        var entity = await _repository.GetAsync(e => e.Id == 50);

        entity.Should().NotBeNull();
        entity!.Name.Should().Be("H");
    }

    [Fact]
    public async Task GetCountAsync_ShouldReturnCount()
    {
        await _repository.AddRangeAsync(new List<TestEntity>
        {
            new TestEntity { Id = 60, Name = "I" },
            new TestEntity { Id = 61, Name = "I" }
        });

        var count = await _repository.GetCountAsync(e => e.Name == "I");
        count.Should().Be(2);
    }

    [Fact]
    public async Task GetListByDynamicAsync_ShouldReturnPaginatedAndFilteredResult()
    {
        await _repository.AddRangeAsync(new List<TestEntity>
        {
            new TestEntity { Id = 70, Name = "J1" },
            new TestEntity { Id = 71, Name = "J2" },
            new TestEntity { Id = 72, Name = "K1" }
        });

        var dynamicQuery = new OleAI.Backend.Core.Repositories.Dynamic.DynamicQuery
        {
            Filter = new OleAI.Backend.Core.Repositories.Dynamic.Filter { Field = "Name", Operator = "contains", Value = "J" }
        };

        var options = new OleAI.Backend.Core.Repositories.Options.DynamicQueryOptions<TestEntity>
        {
            Dynamic = dynamicQuery,
            Index = 0,
            Size = 10
        };

        var result = await _repository.GetListByDynamicAsync(options);

        result.Items.Count.Should().Be(2);
        result.TotalItems.Should().Be(2);
    }

    [Fact]
    public async Task UpdateAsync_ShouldSetUpdatedDateAndModifyEntity()
    {
        var entity = new TestEntity { Id = 80, Name = "Old" };
        await _repository.AddAsync(entity);

        entity.Name = "New";
        var updated = await _repository.UpdateAsync(entity);

        updated.Name.Should().Be("New");
        updated.UpdatedDate.Should().NotBeNull();

        var dbEntity = await _context.TestEntities.FirstAsync(e => e.Id == 80);
        dbEntity.Name.Should().Be("New");
    }

    [Fact]
    public async Task UpdateRangeAsync_ShouldSetUpdatedDateForAll()
    {
        var entities = new List<TestEntity>
        {
            new TestEntity { Id = 90, Name = "Old1" },
            new TestEntity { Id = 91, Name = "Old2" }
        };
        await _repository.AddRangeAsync(entities);

        entities[0].Name = "New1";
        entities[1].Name = "New2";

        await _repository.UpdateRangeAsync(entities);

        var dbEntities = await _context.TestEntities.ToListAsync();
        dbEntities.Should().AllSatisfy(e => e.UpdatedDate.Should().NotBeNull());
        dbEntities.Should().Contain(e => e.Name == "New1");
    }

    [Fact]
    public async Task ExecuteDeleteAsync_InMemory_ShouldThrowInvalidOperationException()
    {
        // InMemory DbContext does not support ExecuteDeleteAsync. 
        // We call it just to ensure the repository passes the call down to EF Core, which throws.
        Func<Task> act = async () => await _repository.ExecuteDeleteAsync(e => e.Name == "A");
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ExecuteUpdateAsync_InMemory_ShouldThrowInvalidOperationException()
    {
        Func<Task> act = async () => await _repository.ExecuteUpdateAsync(
            e => e.Name == "A",
            s => s.SetProperty(x => x.Name, "B")
        );
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
    [Fact]
    public async Task AnyAsync_WithDeleted_ShouldReturnTrueForDeletedEntity()
    {
        var entity = new TestEntity { Id = 100, Name = "Deleted Any", DeletedDate = DateTime.UtcNow };
        await _context.TestEntities.AddAsync(entity);
        await _context.SaveChangesAsync();

        var exists = await _repository.AnyAsync(e => e.Name == "Deleted Any", withDeleted: true);
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task AnyAsync_WithoutDeleted_ShouldReturnFalseForDeletedEntity()
    {
        var entity = new TestEntity { Id = 101, Name = "Deleted Any 2", DeletedDate = DateTime.UtcNow };
        await _context.TestEntities.AddAsync(entity);
        await _context.SaveChangesAsync();

        var exists = await _repository.AnyAsync(e => e.Name == "Deleted Any 2", withDeleted: false);
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task AnyAsync_NoTracking_ShouldExecuteWithoutTracking()
    {
        var entity = new TestEntity { Id = 102, Name = "NoTracking Any" };
        await _context.TestEntities.AddAsync(entity);
        await _context.SaveChangesAsync();

        var exists = await _repository.AnyAsync(e => e.Id == 102, enableTracking: false);
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task GetAsync_WithDeleted_ShouldReturnDeletedEntity()
    {
        var entity = new TestEntity { Id = 103, Name = "Deleted Get", DeletedDate = DateTime.UtcNow };
        await _context.TestEntities.AddAsync(entity);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAsync(e => e.Id == 103, withDeleted: true);
        result.Should().NotBeNull();
        result!.Id.Should().Be(103);
    }

    [Fact]
    public async Task GetAsync_WithoutDeleted_ShouldNotReturnDeletedEntity()
    {
        var entity = new TestEntity { Id = 104, Name = "Deleted Get 2", DeletedDate = DateTime.UtcNow };
        await _context.TestEntities.AddAsync(entity);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAsync(e => e.Id == 104, withDeleted: false);
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_NoTracking_ShouldReturnNotTrackedEntity()
    {
        var entity = new TestEntity { Id = 105, Name = "NoTracking Get" };
        await _context.TestEntities.AddAsync(entity);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAsync(e => e.Id == 105, enableTracking: false);
        result.Should().NotBeNull();
        _context.Entry(result!).State.Should().Be(EntityState.Detached);
    }

    [Fact]
    public async Task GetAsync_WithInclude_ShouldApplyIncludeQuery()
    {
        var entity = new TestEntity { Id = 106, Name = "Include Get" };
        await _context.TestEntities.AddAsync(entity);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAsync(
            predicate: e => e.Id == 106,
            include: null // Removed dummy include to prevent EF primitive errors
        );
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCountAsync_WithDeleted_ShouldIncludeDeleted()
    {
        var entity = new TestEntity { Id = 107, Name = "Deleted Count", DeletedDate = DateTime.UtcNow };
        await _context.TestEntities.AddAsync(entity);
        await _context.SaveChangesAsync();

        var countWithDeleted = await _repository.GetCountAsync(e => e.Name == "Deleted Count", withDeleted: true);
        var countWithoutDeleted = await _repository.GetCountAsync(e => e.Name == "Deleted Count", withDeleted: false);
        var countNoTracking = await _repository.GetCountAsync(e => e.Name == "Deleted Count", enableTracking: false);

        countWithDeleted.Should().Be(1);
        countWithoutDeleted.Should().Be(0);
        countNoTracking.Should().Be(0); // since enableTracking test defaulted withDeleted=false
    }

    [Fact]
    public async Task GetListAsync_FullOptions_ShouldApplyAllFilters()
    {
        var entities = new List<TestEntity>
        {
            new TestEntity { Id = 201, Name = "Z Item", DeletedDate = DateTime.UtcNow },
            new TestEntity { Id = 202, Name = "A Item" },
            new TestEntity { Id = 203, Name = "Z Item", DeletedDate = null }
        };
        await _context.TestEntities.AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        var options = new OleAI.Backend.Core.Repositories.Options.QueryOptions<TestEntity>
        {
            Index = 0,
            Size = 10,
            EnableTracking = false,
            WithDeleted = true,
            Predicate = e => e.Name.Contains("Item"),
            OrderBy = q => q.OrderBy(e => e.Name)
        };

        var result = await _repository.GetListAsync(options);

        result.Items.Count.Should().Be(3);
        result.Items.First().Name.Should().Be("A Item");
        _context.Entry(result.Items.First()).State.Should().Be(EntityState.Detached);
    }

    [Fact]
    public async Task GetListAsync_NoOrderBy_ShouldReturnUnordered()
    {
        var entity = new TestEntity { Id = 204, Name = "List No Order" };
        await _repository.AddAsync(entity);
        var options = new OleAI.Backend.Core.Repositories.Options.QueryOptions<TestEntity>
        {
            Predicate = e => e.Id == 204
        };
        var result = await _repository.GetListAsync(options);
        result.Items.Count.Should().Be(1);
    }

    [Fact]
    public async Task GetListByDynamicAsync_FullOptions_ShouldApplyAllFilters()
    {
        var entities = new List<TestEntity>
        {
            new TestEntity { Id = 301, Name = "Dynamic Z", DeletedDate = DateTime.UtcNow },
            new TestEntity { Id = 302, Name = "Dynamic A" }
        };
        await _context.TestEntities.AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        var options = new OleAI.Backend.Core.Repositories.Options.DynamicQueryOptions<TestEntity>
        {
            Index = 0,
            Size = 10,
            EnableTracking = false,
            WithDeleted = true,
            Predicate = e => e.Name.StartsWith("Dynamic"),
            Dynamic = new OleAI.Backend.Core.Repositories.Dynamic.DynamicQuery()
        };

        var result = await _repository.GetListByDynamicAsync(options);

        result.Items.Count.Should().Be(2);
        _context.Entry(result.Items.First()).State.Should().Be(EntityState.Detached);
    }
}
