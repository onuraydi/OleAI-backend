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

namespace OleAI.Backend.Application.Tests.Features.IngredientAliases.Queries
{
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
        public async Task Handle_ShouldReturnListOfAliasResponses()
        {
            // Arrange
            var query = new GetIngredientAliasListQuery();

            var aliases = new List<IngredientAlias>
            {
                new IngredientAlias { Id = Guid.NewGuid(), AliasName = "Domat" },
                new IngredientAlias { Id = Guid.NewGuid(), AliasName = "Meat" }
            };

            var paginatedAliases = new Paginate<IngredientAlias> { Items = aliases };

            var mappedResponse = new List<IngredientAliasResponse>
            {
                new IngredientAliasResponse { Id = aliases[0].Id, AliasName = "Domat" },
                new IngredientAliasResponse { Id = aliases[1].Id, AliasName = "Meat" }
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
            result.Should().HaveCount(2);
            result[0].AliasName.Should().Be("Domat");
            result[1].AliasName.Should().Be("Meat");

            _mockAliasRepository.Verify(r => r.GetListAsync(
                It.Is<QueryOptions<IngredientAlias>>(opt => opt.EnableTracking == false),
                It.IsAny<CancellationToken>()), Times.Once);
            
            _mockMapper.Verify(m => m.Map<List<IngredientAliasResponse>>(aliases), Times.Once);
        }
    }
}
