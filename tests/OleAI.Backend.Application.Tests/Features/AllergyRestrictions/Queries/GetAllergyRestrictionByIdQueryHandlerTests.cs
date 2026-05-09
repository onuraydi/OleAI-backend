using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.AllergyRestrictions.Dtos.Responses;
using OleAI.Backend.Application.Features.AllergyRestrictions.Queries.GetAllergyRestrictionById;
using OleAI.Backend.Application.Features.AllergyRestrictions.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.AllergyRestrictions.Queries.GetAllergyRestrictionById;

public class GetAllergyRestrictionByIdQueryHandlerTests
{
    private readonly Mock<IAllergyRestrictionRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly AllergyRestrictionBusinessRules _businessRules;
    private readonly GetAllergyRestrictionByIdQueryHandler _handler;

    public GetAllergyRestrictionByIdQueryHandlerTests()
    {
        _mockRepository = new Mock<IAllergyRestrictionRepository>();
        _mockMapper = new Mock<IMapper>();
        _businessRules = new AllergyRestrictionBusinessRules(_mockRepository.Object);
        _handler = new GetAllergyRestrictionByIdQueryHandler(_mockRepository.Object, _businessRules, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_WhenExists_ShouldReturnDetailResponse()
    {
        // Arrange
        var query = new GetAllergyRestrictionByIdQuery { Id = Guid.NewGuid() };
        var restriction = new AllergyRestriction { Id = query.Id, Name = "Nut Allergy" };
        var response = new AllergyRestrictionDetailResponse { Id = query.Id, Name = "Nut Allergy" };

        _mockRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<AllergyRestriction, bool>>>(), It.IsAny<Func<IQueryable<AllergyRestriction>, IIncludableQueryable<AllergyRestriction, object>>>(), false, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(restriction);

        _mockMapper.Setup(m => m.Map<AllergyRestrictionDetailResponse>(restriction)).Returns(response);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(query.Id);
        result.Name.Should().Be("Nut Allergy");
    }
}
