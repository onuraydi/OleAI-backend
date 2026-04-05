using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Responses;
using OleAI.Backend.Application.Features.IngredientAliases.Queries.GetIngredientAliasById;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.IngredientAliases.Queries.GetIngredientAliasById;

public class GetIngredientAliasByIdQueryHandlerTests
{
    private readonly Mock<IIngredientAliasRepository> _mockAliasRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetIngredientAliasByIdQueryHandler _handler;

    public GetIngredientAliasByIdQueryHandlerTests()
    {
        _mockAliasRepository = new Mock<IIngredientAliasRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetIngredientAliasByIdQueryHandler(_mockAliasRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_WhenAliasExists_ShouldReturnMappedResponse()
    {
        var alias = new IngredientAlias(Guid.NewGuid(), Guid.NewGuid(), "Domat");
        var query = new GetIngredientAliasByIdQuery { Id = alias.Id };

        var response = new IngredientAliasResponse { Id = alias.Id, IngredientId = alias.IngredientId, AliasName = alias.AliasName };

        _mockAliasRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<IngredientAlias, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(alias);

        _mockMapper
            .Setup(m => m.Map<IngredientAliasResponse>(alias))
            .Returns(response);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.AliasName.Should().Be("Domat");
    }
}
