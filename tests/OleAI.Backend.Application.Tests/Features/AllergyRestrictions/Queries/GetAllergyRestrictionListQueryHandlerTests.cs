using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.AllergyRestrictions.Dtos.Responses;
using OleAI.Backend.Application.Features.AllergyRestrictions.Queries.GetAllergyRestrictionList;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.AllergyRestrictions.Queries.GetAllergyRestrictionList;

public class GetAllergyRestrictionListQueryHandlerTests
{
    private readonly Mock<IAllergyRestrictionRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetAllergyRestrictionListQueryHandler _handler;

    public GetAllergyRestrictionListQueryHandlerTests()
    {
        _mockRepository = new Mock<IAllergyRestrictionRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetAllergyRestrictionListQueryHandler(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnListResponse()
    {
        // Arrange
        var query = new GetAllergyRestrictionListQuery();
        var restrictions = new List<AllergyRestriction> { new AllergyRestriction { Id = Guid.NewGuid(), Name = "Nut Allergy" } };
        var paginate = new Paginate<AllergyRestriction> { Items = restrictions };
        var responseList = new List<AllergyRestrictionListResponse> { new AllergyRestrictionListResponse { Name = "Nut Allergy" } };

        _mockRepository
            .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<AllergyRestriction>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginate);

        _mockMapper.Setup(m => m.Map<List<AllergyRestrictionListResponse>>(restrictions)).Returns(responseList);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(1);
        result[0].Name.Should().Be("Nut Allergy");
    }
}
