using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Responses;
using OleAI.Backend.Application.Features.IngredientAliases.Queries.GetIngredientAliasList;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.IngredientAliases.Queries.GetIngredientAliasList;

public class GetIngredientAliasListQueryHandlerTests
{
    private readonly Mock<IIngredientAliasRepository> _mockAliasRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetIngredientAliasListQueryHandler _handler;

    public GetIngredientAliasListQueryHandlerTests()
    {
        _mockAliasRepository = new Mock<IIngredientAliasRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetIngredientAliasListQueryHandler(_mockAliasRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ShouldQueryWithNoTracking_AndReturnMappedList()
    {
        var aliases = new List<IngredientAlias>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), "Domat"),
            new(Guid.NewGuid(), Guid.NewGuid(), "Patlıcan"),
        };

        var mapped = new List<IngredientAliasResponse>
        {
            new() { Id = aliases[0].Id, IngredientId = aliases[0].IngredientId, AliasName = aliases[0].AliasName },
            new() { Id = aliases[1].Id, IngredientId = aliases[1].IngredientId, AliasName = aliases[1].AliasName },
        };

        _mockAliasRepository
            .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<IngredientAlias>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Paginate<IngredientAlias> { Items = aliases });

        _mockMapper
            .Setup(m => m.Map<List<IngredientAliasResponse>>(aliases))
            .Returns(mapped);

        var result = await _handler.Handle(new GetIngredientAliasListQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        _mockAliasRepository.Verify(r => r.GetListAsync(
            It.Is<QueryOptions<IngredientAlias>>(q => q.EnableTracking == false),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
