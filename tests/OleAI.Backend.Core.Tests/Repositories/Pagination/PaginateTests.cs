using FluentAssertions;
using OleAI.Backend.Core.Repositories.Pagination;

namespace OleAI.Backend.Core.Tests.Repositories.Pagination;

public class PaginateTests
{
    [Fact]
    public void DefaultConstructor_ShouldInitializeEmptyItems()
    {
        var paginate = new Paginate<int>();

        paginate.Items.Should().NotBeNull();
        paginate.Items.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_ShouldCalculatePagingValues()
    {
        var items = new List<int> { 1, 2, 3 };
        var pagination = new PaginationParams { PageNumber = 2, PageSize = 3 };

        var paginate = new Paginate<int>(items, totalItems: 8, pagination);

        paginate.PageNumber.Should().Be(2);
        paginate.PageSize.Should().Be(3);
        paginate.TotalItems.Should().Be(8);
        paginate.TotalPages.Should().Be(3);
        paginate.HasPreviousPage.Should().BeTrue();
        paginate.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public void HasNextPage_ShouldBeFalse_OnLastPage()
    {
        var paginate = new Paginate<int>(new List<int> { 1 }, totalItems: 1, new PaginationParams { PageNumber = 1, PageSize = 10 });

        paginate.HasPreviousPage.Should().BeFalse();
        paginate.HasNextPage.Should().BeFalse();
    }
}
