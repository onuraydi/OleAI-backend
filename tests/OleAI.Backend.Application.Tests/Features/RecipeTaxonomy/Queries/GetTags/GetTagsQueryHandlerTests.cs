using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.RecipeTaxonomy.Queries.GetTags;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.RecipeTaxonomy.Queries.GetTags;

public class GetTagsQueryHandlerTests
{
    private readonly Mock<ITagRepository> _tagRepositoryMock;
    private readonly GetTagsQueryHandler _handler;

    public GetTagsQueryHandlerTests()
    {
        _tagRepositoryMock = new Mock<ITagRepository>();

        _handler = new GetTagsQueryHandler(_tagRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTagsList()
    {
        // Arrange
        var query = new GetTagsQuery();

        var tags = new List<OleAI.Backend.Domain.Entities.Tag>
        {
            new OleAI.Backend.Domain.Entities.Tag(1, "Vegan"),
            new OleAI.Backend.Domain.Entities.Tag(2, "Gluten Free")
        };

        var paginatedTags = new Paginate<OleAI.Backend.Domain.Entities.Tag>
        {
            Items = tags,
            PageNumber = 1,
            PageSize = 1000,
            TotalItems = 2,
            TotalPages = 1
        };

        _tagRepositoryMock.Setup(x => x.GetListAsync(
                It.IsAny<QueryOptions<OleAI.Backend.Domain.Entities.Tag>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedTags);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        // Results are sorted by Name (Gluten Free then Vegan)
        result[0].Name.Should().Be("Gluten Free");
        result[1].Name.Should().Be("Vegan");

        _tagRepositoryMock.Verify(x => x.GetListAsync(
            It.Is<QueryOptions<OleAI.Backend.Domain.Entities.Tag>>(q => q.Size == 1000 && q.EnableTracking == false),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
