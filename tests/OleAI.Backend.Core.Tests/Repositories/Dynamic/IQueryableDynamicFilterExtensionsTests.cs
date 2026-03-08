using FluentAssertions;
using OleAI.Backend.Core.Repositories.Dynamic;
using Xunit;

namespace OleAI.Backend.Core.Tests.Repositories.Dynamic;

public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? Age { get; set; }
}

public class IQueryableDynamicFilterExtensionsTests
{
    private readonly IQueryable<TestEntity> _queryable;

    public IQueryableDynamicFilterExtensionsTests()
    {
        var data = new List<TestEntity>
        {
            new TestEntity { Id = 1, Name = "Alice", Age = 25 },
            new TestEntity { Id = 2, Name = "Bob", Age = 30 },
            new TestEntity { Id = 3, Name = "Charlie", Age = 35 },
            new TestEntity { Id = 4, Name = "David", Age = null }
        };
        _queryable = data.AsQueryable();
    }

    [Fact]
    public void ToDynamic_WithFilterEq_ShouldReturnFilteredData()
    {
        // Arrange
        var dynamicQuery = new DynamicQuery
        {
            Filter = new Filter { Field = "Name", Operator = "eq", Value = "Bob" }
        };

        // Act
        var result = _queryable.ToDynamic(dynamicQuery).ToList();

        // Assert
        result.Count.Should().Be(1);
        result.First().Name.Should().Be("Bob");
    }

    [Fact]
    public void ToDynamic_WithFilterContains_ShouldReturnFilteredData()
    {
        // Arrange
        var dynamicQuery = new DynamicQuery
        {
            Filter = new Filter { Field = "Name", Operator = "contains", Value = "li" }
        };

        // Act
        var result = _queryable.ToDynamic(dynamicQuery).ToList();

        // Assert
        result.Count.Should().Be(2);
        result.Should().Contain(e => e.Name == "Alice");
        result.Should().Contain(e => e.Name == "Charlie");
    }

    [Fact]
    public void ToDynamic_WithFilterIsNull_ShouldReturnFilteredData()
    {
        // Arrange
        var dynamicQuery = new DynamicQuery
        {
            Filter = new Filter { Field = "Age", Operator = "isnull" }
        };

        // Act
        var result = _queryable.ToDynamic(dynamicQuery).ToList();

        // Assert
        result.Count.Should().Be(1);
        result.First().Name.Should().Be("David");
    }

    [Fact]
    public void ToDynamic_WithSortDesc_ShouldReturnSortedData()
    {
        // Arrange
        var dynamicQuery = new DynamicQuery
        {
            Sort = new List<Sort> { new Sort { Field = "Name", Dir = "desc" } }
        };

        // Act
        var result = _queryable.ToDynamic(dynamicQuery).ToList();

        // Assert
        result.First().Name.Should().Be("David");
        result.Last().Name.Should().Be("Alice");
    }

    [Fact]
    public void ToDynamic_WithCompoundLogic_ShouldReturnFilteredData()
    {
        // Arrange
        var dynamicQuery = new DynamicQuery
        {
            Filter = new Filter
            {
                Field = "Name",
                Operator = "eq",
                Value = "Alice",
                Logic = "or",
                Filters = new List<Filter>
                {
                    new Filter { Field = "Name", Operator = "eq", Value = "Charlie" }
                }
            }
        };

        // Act
        var result = _queryable.ToDynamic(dynamicQuery).ToList();

        // Assert
        result.Count.Should().Be(2);
        result.Should().Contain(e => e.Name == "Alice");
        result.Should().Contain(e => e.Name == "Charlie");
    }
}
