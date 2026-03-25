using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Core.Repositories.Dynamic;
using Xunit;

namespace OleAI.Backend.Core.Tests.Repositories.Dynamic;

public class FilterEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string? Description { get; set; }
}

public class FilterDbContext : DbContext
{
    public FilterDbContext(DbContextOptions<FilterDbContext> options) : base(options)
    {
    }
    public DbSet<FilterEntity> FilterEntities { get; set; }
}

public class IQueryableDynamicFilterExtensionsTests
{
    private readonly FilterDbContext _context;

    public IQueryableDynamicFilterExtensionsTests()
    {
        var options = new DbContextOptionsBuilder<FilterDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new FilterDbContext(options);
    }

    private async Task SeedData()
    {
        await _context.FilterEntities.AddRangeAsync(new List<FilterEntity>
        {
            new FilterEntity { Id = 1, Name = "Alice", Age = 25, Description = "Engineer" },
            new FilterEntity { Id = 2, Name = "Bob", Age = 30, Description = "Manager" },
            new FilterEntity { Id = 3, Name = "Charlie", Age = 35, Description = null },
            new FilterEntity { Id = 4, Name = "David", Age = 40, Description = "Director" },
            new FilterEntity { Id = 5, Name = "Eve", Age = 45, Description = "Robot" }
        });
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task ToDynamic_Operator_Eq_ShouldWork()
    {
        await SeedData();
        var dynamicQuery = new DynamicQuery { Filter = new Filter { Field = "Name", Operator = "eq", Value = "Alice" } };
        var result = _context.FilterEntities.ToDynamic(dynamicQuery).ToList();
        result.Should().ContainSingle().Which.Name.Should().Be("Alice");
    }

    [Fact]
    public async Task ToDynamic_Operator_Neq_ShouldWork()
    {
        await SeedData();
        var dynamicQuery = new DynamicQuery { Filter = new Filter { Field = "Age", Operator = "neq", Value = "30" } };
        var result = _context.FilterEntities.ToDynamic(dynamicQuery).ToList();
        result.Count.Should().Be(4);
        result.Should().NotContain(x => x.Age == 30);
    }

    [Fact]
    public async Task ToDynamic_Operator_Lt_ShouldWork()
    {
        await SeedData();
        var dynamicQuery = new DynamicQuery { Filter = new Filter { Field = "Age", Operator = "lt", Value = "30" } };
        var result = _context.FilterEntities.ToDynamic(dynamicQuery).ToList();
        result.Should().ContainSingle().Which.Name.Should().Be("Alice");
    }

    [Fact]
    public async Task ToDynamic_Operator_Lte_ShouldWork()
    {
        await SeedData();
        var dynamicQuery = new DynamicQuery { Filter = new Filter { Field = "Age", Operator = "lte", Value = "30" } };
        var result = _context.FilterEntities.ToDynamic(dynamicQuery).ToList();
        result.Count.Should().Be(2);
    }

    [Fact]
    public async Task ToDynamic_Operator_Gt_ShouldWork()
    {
        await SeedData();
        var dynamicQuery = new DynamicQuery { Filter = new Filter { Field = "Age", Operator = "gt", Value = "35" } };
        var result = _context.FilterEntities.ToDynamic(dynamicQuery).ToList();
        result.Count.Should().Be(2); // David (40), Eve (45)
    }

    [Fact]
    public async Task ToDynamic_Operator_Gte_ShouldWork()
    {
        await SeedData();
        var dynamicQuery = new DynamicQuery { Filter = new Filter { Field = "Age", Operator = "gte", Value = "35" } };
        var result = _context.FilterEntities.ToDynamic(dynamicQuery).ToList();
        result.Count.Should().Be(3); // Charlie, David, Eve
    }

    [Fact]
    public async Task ToDynamic_Operator_IsNull_ShouldWork()
    {
        await SeedData();
        var dynamicQuery = new DynamicQuery { Filter = new Filter { Field = "Description", Operator = "isnull" } };
        var result = _context.FilterEntities.ToDynamic(dynamicQuery).ToList();
        result.Should().ContainSingle().Which.Name.Should().Be("Charlie");
    }

    [Fact]
    public async Task ToDynamic_Operator_IsNotNull_ShouldWork()
    {
        await SeedData();
        var dynamicQuery = new DynamicQuery { Filter = new Filter { Field = "Description", Operator = "isnotnull" } };
        var result = _context.FilterEntities.ToDynamic(dynamicQuery).ToList();
        result.Count.Should().Be(4);
    }

    [Fact]
    public async Task ToDynamic_Operator_StartsWith_ShouldWork()
    {
        await SeedData();
        var dynamicQuery = new DynamicQuery { Filter = new Filter { Field = "Name", Operator = "startswith", Value = "Bo" } };
        var result = _context.FilterEntities.ToDynamic(dynamicQuery).ToList();
        result.Should().ContainSingle().Which.Name.Should().Be("Bob");
    }

    [Fact]
    public async Task ToDynamic_Operator_EndsWith_ShouldWork()
    {
        await SeedData();
        var dynamicQuery = new DynamicQuery { Filter = new Filter { Field = "Name", Operator = "endswith", Value = "ie" } };
        var result = _context.FilterEntities.ToDynamic(dynamicQuery).ToList();
        result.Should().ContainSingle().Which.Name.Should().Be("Charlie");
    }

    [Fact]
    public async Task ToDynamic_Operator_Contains_ShouldWork()
    {
        await SeedData();
        var dynamicQuery = new DynamicQuery { Filter = new Filter { Field = "Description", Operator = "contains", Value = "nag" } };
        var result = _context.FilterEntities.ToDynamic(dynamicQuery).ToList();
        result.Should().ContainSingle().Which.Description.Should().Be("Manager");
    }

    [Fact]
    public async Task ToDynamic_Operator_DoesNotContain_ShouldWork()
    {
        await SeedData();
        var dynamicQuery = new DynamicQuery { Filter = new Filter { Field = "Description", Operator = "doesnotcontain", Value = "e" } };
        var result = _context.FilterEntities.ToDynamic(dynamicQuery).ToList();
        // Alice, Bob, David contain 'e'. Eve (Robot) does not. Charlie is null.
        result.Should().Contain(x => x.Name == "Eve");
        result.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ToDynamic_Logic_And_ShouldWork()
    {
        await SeedData();
        var dynamicQuery = new DynamicQuery
        {
            Filter = new Filter
            {
                Logic = "and",
                Filters = new List<Filter>
                {
                    new Filter { Field = "Age", Operator = "gt", Value = "25" },
                    new Filter { Field = "Age", Operator = "lt", Value = "40" }
                }
            }
        };
        var result = _context.FilterEntities.ToDynamic(dynamicQuery).ToList();
        result.Count.Should().Be(2); // Bob (30), Charlie (35)
    }

    [Fact]
    public async Task ToDynamic_Logic_Or_ShouldWork()
    {
        await SeedData();
        var dynamicQuery = new DynamicQuery
        {
            Filter = new Filter
            {
                Logic = "or",
                Filters = new List<Filter>
                {
                    new Filter { Field = "Name", Operator = "eq", Value = "Alice" },
                    new Filter { Field = "Name", Operator = "eq", Value = "Bob" }
                }
            }
        };
        var result = _context.FilterEntities.ToDynamic(dynamicQuery).ToList();
        result.Count.Should().Be(2);
    }

    [Fact]
    public async Task ToDynamic_Sort_ShouldWork()
    {
        await SeedData();
        var dynamicQuery = new DynamicQuery
        {
            Sort = new List<Sort>
            {
                new Sort { Field = "Age", Dir = "desc" }
            }
        };
        var result = _context.FilterEntities.ToDynamic(dynamicQuery).ToList();
        result.First().Name.Should().Be("Eve");
        result.Last().Name.Should().Be("Alice");
    }
}
