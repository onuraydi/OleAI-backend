using FluentAssertions;
using OleAI.Backend.Core.Repositories.Dynamic;
using OleAI.Backend.Core.Repositories.Options;

namespace OleAI.Backend.Core.Tests.Repositories.Options;

public class QueryOptionsTests
{
    [Fact]
    public void BaseQueryOptions_ShouldHaveExpectedDefaults()
    {
        var options = new BaseQueryOptions<int>();

        options.Predicate.Should().BeNull();
        options.Include.Should().BeNull();
        options.Index.Should().Be(0);
        options.Size.Should().Be(10);
        options.WithDeleted.Should().BeFalse();
        options.EnableTracking.Should().BeTrue();
    }

    [Fact]
    public void QueryOptions_ShouldAllowOrderByAssignment()
    {
        var options = new QueryOptions<int>
        {
            OrderBy = query => query.OrderBy(x => x)
        };

        options.OrderBy.Should().NotBeNull();
    }

    [Fact]
    public void DynamicQueryOptions_ShouldStoreDynamicQuery()
    {
        var dynamic = new DynamicQuery
        {
            Filter = new Filter { Field = "Quantity", Operator = "gte", Value = "10" },
            Sort = new List<Sort> { new() { Field = "Name", Dir = "asc" } }
        };

        var options = new DynamicQueryOptions<int>
        {
            Dynamic = dynamic
        };

        options.Dynamic.Should().Be(dynamic);
        options.Dynamic.Filter!.Field.Should().Be("Quantity");
    }
}
