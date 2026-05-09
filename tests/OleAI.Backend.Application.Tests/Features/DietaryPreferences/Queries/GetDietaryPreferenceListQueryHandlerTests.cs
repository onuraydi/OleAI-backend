using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.DietaryPreferences.Dtos.Responses;
using OleAI.Backend.Application.Features.DietaryPreferences.Queries.GetDietaryPreferenceList;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.DietaryPreferences.Queries.GetDietaryPreferenceList;

public class GetDietaryPreferenceListQueryHandlerTests
{
    private readonly Mock<IDietaryPreferenceRepository> _mockDietaryPreferenceRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetDietaryPreferenceListQueryHandler _handler;

    public GetDietaryPreferenceListQueryHandlerTests()
    {
        _mockDietaryPreferenceRepository = new Mock<IDietaryPreferenceRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetDietaryPreferenceListQueryHandler(_mockDietaryPreferenceRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnListResponse()
    {
        // Arrange
        var query = new GetDietaryPreferenceListQuery();
        var preferences = new List<DietaryPreference> { new DietaryPreference { Id = Guid.NewGuid(), Name = "Vegan" } };
        var paginate = new Paginate<DietaryPreference> { Items = preferences };
        var responseList = new List<DietaryPreferenceListResponse> { new DietaryPreferenceListResponse { Name = "Vegan" } };

        _mockDietaryPreferenceRepository
            .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<DietaryPreference>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginate);

        _mockMapper.Setup(m => m.Map<List<DietaryPreferenceListResponse>>(preferences)).Returns(responseList);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(1);
        result[0].Name.Should().Be("Vegan");
    }
}
