using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Pantry.Dtos.Responses;
using OleAI.Backend.Application.Features.Pantry.Queries.GetResolvedRequests;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Pantry.Queries;

public class GetResolvedIngredientRequestsQueryHandlerTests
{
    private readonly Mock<IIngredientRequestRepository> _mockIngredientRequestRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetResolvedIngredientRequestsQueryHandler _handler;

    public GetResolvedIngredientRequestsQueryHandlerTests()
    {
        _mockIngredientRequestRepository = new Mock<IIngredientRequestRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetResolvedIngredientRequestsQueryHandler(_mockIngredientRequestRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ShouldPassPaginationToRepository_AndReturnMappedPaginate()
    {
        // Arrange
        var query = new GetResolvedIngredientRequestsQuery { Page = 2, Size = 5 };

        var entities = new List<IngredientRequest>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), "Domat")
        };
        entities[0].ResolveAsApproved();

        var repositoryResult = new Paginate<IngredientRequest>
        {
            Items = entities,
            PageNumber = 2,
            PageSize = 5,
            TotalItems = 11,
            TotalPages = 3
        };

        var mapped = new List<ResolvedRequestsResponse>
        {
            new() { Id = entities[0].Id, UserId = entities[0].UserId, RequestedName = entities[0].RequestedName }
        };

        _mockIngredientRequestRepository
            .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<IngredientRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(repositoryResult);

        _mockMapper
            .Setup(m => m.Map<List<ResolvedRequestsResponse>>(entities))
            .Returns(mapped);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().BeEquivalentTo(mapped);
        result.PageNumber.Should().Be(2);
        result.PageSize.Should().Be(5);
        result.TotalItems.Should().Be(11);
        result.TotalPages.Should().Be(3);

        _mockIngredientRequestRepository.Verify(r => r.GetListAsync(
            It.Is<QueryOptions<IngredientRequest>>(q => q.Index == 2 && q.Size == 5 && q.EnableTracking == false),
            It.IsAny<CancellationToken>()), Times.Once);

        _mockMapper.Verify(m => m.Map<List<ResolvedRequestsResponse>>(entities), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNoResolvedRequests_ShouldReturnEmptyPaginate()
    {
        // Arrange
        var query = new GetResolvedIngredientRequestsQuery { Page = 0, Size = 20 };

        var repositoryResult = new Paginate<IngredientRequest>
        {
            Items = new List<IngredientRequest>(),
            PageNumber = 0,
            PageSize = 20,
            TotalItems = 0,
            TotalPages = 0
        };

        _mockIngredientRequestRepository
            .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<IngredientRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(repositoryResult);

        _mockMapper
            .Setup(m => m.Map<List<ResolvedRequestsResponse>>(repositoryResult.Items))
            .Returns(new List<ResolvedRequestsResponse>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalItems.Should().Be(0);
    }
}
