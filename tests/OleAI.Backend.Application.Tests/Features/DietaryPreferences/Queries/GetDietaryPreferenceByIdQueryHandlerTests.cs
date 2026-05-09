using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.DietaryPreferences.Dtos.Responses;
using OleAI.Backend.Application.Features.DietaryPreferences.Queries.GetDietaryPreferenceById;
using OleAI.Backend.Application.Features.DietaryPreferences.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.DietaryPreferences.Queries.GetDietaryPreferenceById;

public class GetDietaryPreferenceByIdQueryHandlerTests
{
    private readonly Mock<IDietaryPreferenceRepository> _mockDietaryPreferenceRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly DietaryPreferenceBusinessRules _businessRules;
    private readonly GetDietaryPreferenceByIdQueryHandler _handler;

    public GetDietaryPreferenceByIdQueryHandlerTests()
    {
        _mockDietaryPreferenceRepository = new Mock<IDietaryPreferenceRepository>();
        _mockMapper = new Mock<IMapper>();
        _businessRules = new DietaryPreferenceBusinessRules(_mockDietaryPreferenceRepository.Object);
        _handler = new GetDietaryPreferenceByIdQueryHandler(_mockDietaryPreferenceRepository.Object, _businessRules, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_WhenExists_ShouldReturnDetailResponse()
    {
        // Arrange
        var query = new GetDietaryPreferenceByIdQuery { Id = Guid.NewGuid() };
        var preference = new DietaryPreference { Id = query.Id, Name = "Vegan" };
        var response = new DietaryPreferenceDetailResponse { Id = query.Id, Name = "Vegan" };

        _mockDietaryPreferenceRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<DietaryPreference, bool>>>(), It.IsAny<Func<IQueryable<DietaryPreference>, IIncludableQueryable<DietaryPreference, object>>>(), false, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        _mockMapper.Setup(m => m.Map<DietaryPreferenceDetailResponse>(preference)).Returns(response);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(query.Id);
        result.Name.Should().Be("Vegan");
    }
}
