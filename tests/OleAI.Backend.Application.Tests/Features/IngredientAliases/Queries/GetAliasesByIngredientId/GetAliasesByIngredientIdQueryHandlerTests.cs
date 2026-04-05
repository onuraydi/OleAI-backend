using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Responses;
using OleAI.Backend.Application.Features.IngredientAliases.Queries.GetAliasesByIngredientId;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.IngredientAliases.Queries.GetAliasesByIngredientId;

public class GetAliasesByIngredientIdQueryHandlerTests
{
    private readonly Mock<IIngredientAliasRepository> _mockAliasRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetAliasesByIngredientIdQueryHandler _handler;

    public GetAliasesByIngredientIdQueryHandlerTests()
    {
        _mockAliasRepository = new Mock<IIngredientAliasRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetAliasesByIngredientIdQueryHandler(_mockAliasRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ShouldFilterByIngredientId_AndReturnMappedAliases()
    {
        var ingredientId = Guid.NewGuid();
        var aliases = new List<IngredientAlias>
        {
            new(Guid.NewGuid(), ingredientId, "Domat"),
            new(Guid.NewGuid(), ingredientId, "Dometes")
        };

        var mapped = new List<IngredientAliasResponse>
        {
            new() { Id = aliases[0].Id, IngredientId = ingredientId, AliasName = "Domat" },
            new() { Id = aliases[1].Id, IngredientId = ingredientId, AliasName = "Dometes" }
        };

        _mockAliasRepository
            .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<IngredientAlias>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Paginate<IngredientAlias> { Items = aliases });

        _mockMapper
            .Setup(m => m.Map<List<IngredientAliasResponse>>(aliases))
            .Returns(mapped);

        var result = await _handler.Handle(new GetAliasesByIngredientIdQuery { IngredientId = ingredientId }, CancellationToken.None);

        result.Should().HaveCount(2);
        result.All(x => x.IngredientId == ingredientId).Should().BeTrue();

        _mockAliasRepository.Verify(r => r.GetListAsync(
            It.Is<QueryOptions<IngredientAlias>>(q => q.EnableTracking == false),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
