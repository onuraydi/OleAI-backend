using FluentAssertions;
using OleAI.Backend.Core.Repositories.Dynamic;

namespace OleAI.Backend.Core.Tests.Repositories.Dynamic;

public class DynamicQueryModelsTests
{
    [Fact]
    public void DynamicQuery_ShouldAllowNestedFilterAndSort()
    {
        var nestedFilter = new Filter
        {
            Field = "CategoryId",
            Operator = "eq",
            Value = "abc"
        };

        var filter = new Filter
        {
            Field = "Quantity",
            Operator = "gte",
            Value = "10",
            Logic = "and",
            Filters = new List<Filter> { nestedFilter }
        };

        var sort = new Sort { Field = "Name", Dir = "desc" };

        var query = new DynamicQuery
        {
            Filter = filter,
            Sort = new List<Sort> { sort }
        };

        query.Filter!.Field.Should().Be("Quantity");
        query.Filter.Filters.Should().ContainSingle();
        query.Sort.Should().ContainSingle();
        query.Sort!.First().Dir.Should().Be("desc");
    }
}
