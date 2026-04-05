using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Responses;
using OleAI.Backend.Application.Features.IngredientAliases.Queries.GetIngredientAliasById;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using System.Linq.Expressions;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;

namespace OleAI.Backend.Application.Tests.Features.IngredientAliases.Queries
{
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
        public async Task Handle_WhenAliasExists_ShouldReturnResponse()
        {
            // Arrange
            var aliasId = Guid.NewGuid();
            var query = new GetIngredientAliasByIdQuery { Id = aliasId };
            var existingAlias = new IngredientAlias { Id = aliasId, AliasName = "Domat" };
            var expectedResponse = new IngredientAliasResponse { Id = aliasId, AliasName = "Domat" };

            _mockAliasRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<IngredientAlias, bool>>>(),
                    null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingAlias);

            _mockMapper.Setup(m => m.Map<IngredientAliasResponse>(existingAlias))
                .Returns(expectedResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(aliasId);
            result.AliasName.Should().Be("Domat");

            _mockAliasRepository.Verify(r => r.GetAsync(
                It.IsAny<Expression<Func<IngredientAlias, bool>>>(),
                null, false, true, It.IsAny<CancellationToken>()), Times.Once);
            _mockMapper.Verify(m => m.Map<IngredientAliasResponse>(existingAlias), Times.Once);
        }
    }
}
