using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Pantry.Dtos.Requests;
using OleAI.Backend.Application.Features.Pantry.Queries.GetPendingRequests;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Pantry.Queries;

public class GetPendingIngredientRequestsQueryHandlerTests
{
    private readonly Mock<IIngredientRequestRepository> _mockIngredientRequestRepository;
    private readonly GetPendingIngredientRequestsQueryHandler _handler;

    public GetPendingIngredientRequestsQueryHandlerTests()
    {
        _mockIngredientRequestRepository = new Mock<IIngredientRequestRepository>();
        _handler = new GetPendingIngredientRequestsQueryHandler(_mockIngredientRequestRepository.Object);
    }

    [Fact]
    public async Task Handle_ShouldQueryPendingRequestsWithoutTracking_AndMapResult()
    {
        // Arrange
        var pending = new IngredientRequest(Guid.NewGuid(), Guid.NewGuid(), "Domates");
        pending.UpdateLlmAnalysis("{\"IsNew\":false}");

        _mockIngredientRequestRepository
            .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<IngredientRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Paginate<IngredientRequest> { Items = new List<IngredientRequest> { pending } });

        // Act
        var result = await _handler.Handle(new GetPendingIngredientRequestsQuery(), CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].Id.Should().Be(pending.Id);
        result[0].UserId.Should().Be(pending.UserId);

        _mockIngredientRequestRepository.Verify(r => r.GetListAsync(
            It.Is<QueryOptions<IngredientRequest>>(q => q.EnableTracking == false),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsNoData_ShouldReturnEmptyList()
    {
        // Arrange
        _mockIngredientRequestRepository
            .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<IngredientRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Paginate<IngredientRequest> { Items = new List<IngredientRequest>() });

        // Act
        var result = await _handler.Handle(new GetPendingIngredientRequestsQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
