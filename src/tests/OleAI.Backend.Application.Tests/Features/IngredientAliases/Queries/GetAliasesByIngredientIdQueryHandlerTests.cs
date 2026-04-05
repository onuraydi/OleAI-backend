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

namespace OleAI.Backend.Application.Tests.Features.IngredientAliases.Queries
{
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
        public async Task Handle_ShouldReturnListOfAliasResponsesForIngredientId()
        {
            // Arrange
            var ingredientId = Guid.NewGuid();
            var query = new GetAliasesByIngredientIdQuery { IngredientId = ingredientId };

            var aliases = new List<IngredientAlias>
            {
                new IngredientAlias { Id = Guid.NewGuid(), IngredientId = ingredientId, AliasName = "Domat" }
            };

            var paginatedAliases = new Paginate<IngredientAlias> { Items = aliases };

            var mappedResponse = new List<IngredientAliasResponse>
            {
                new IngredientAliasResponse { Id = aliases[0].Id, IngredientId = ingredientId, AliasName = "Domat" }
            };

            _mockAliasRepository
                .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<IngredientAlias>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(paginatedAliases);

            _mockMapper
                .Setup(m => m.Map<List<IngredientAliasResponse>>(aliases))
                .Returns(mappedResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].AliasName.Should().Be("Domat");

            _mockAliasRepository.Verify(r => r.GetListAsync(
                It.Is<QueryOptions<IngredientAlias>>(opt => opt.EnableTracking == false),
                It.IsAny<CancellationToken>()), Times.Once);
            
            _mockMapper.Verify(m => m.Map<List<IngredientAliasResponse>>(aliases), Times.Once);
        }
    }
}
