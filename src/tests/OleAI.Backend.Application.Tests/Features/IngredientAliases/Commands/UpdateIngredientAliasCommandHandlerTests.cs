using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.IngredientAliases.Commands.UpdateIngredientAlias;
using OleAI.Backend.Application.Features.IngredientAliases.Constants;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Requests;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Responses;
using OleAI.Backend.Application.Features.IngredientAliases.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using System.Linq.Expressions;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;

namespace OleAI.Backend.Application.Tests.Features.IngredientAliases.Commands
{
    public class UpdateIngredientAliasCommandHandlerTests
    {
        private readonly Mock<IIngredientAliasRepository> _mockAliasRepository;
        private readonly Mock<IIngredientRepository> _mockIngredientRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly IngredientAliasBusinessRules _businessRules;
        private readonly UpdateIngredientAliasCommandHandler _handler;

        public UpdateIngredientAliasCommandHandlerTests()
        {
            _mockAliasRepository = new Mock<IIngredientAliasRepository>();
            _mockIngredientRepository = new Mock<IIngredientRepository>();
            _mockMapper = new Mock<IMapper>();
            _businessRules = new IngredientAliasBusinessRules(_mockAliasRepository.Object, _mockIngredientRepository.Object);

            _handler = new UpdateIngredientAliasCommandHandler(
                _mockAliasRepository.Object,
                _mockMapper.Object,
                _businessRules);
        }

        [Fact]
        public async Task Handle_WhenValidRequest_ShouldUpdateAliasAndReturnResponse()
        {
            // Arrange
            var aliasId = Guid.NewGuid();
            var ingredientId = Guid.NewGuid();
            var request = new UpdateIngredientAliasRequest { Id = aliasId, AliasName = "Domatesik" };
            var command = new UpdateIngredientAliasCommand { Request = request };

            var existingAlias = new IngredientAlias { Id = aliasId, IngredientId = ingredientId, AliasName = "Domat" };
            var mappedAlias = new IngredientAlias { Id = aliasId, IngredientId = ingredientId, AliasName = "Domatesik" };
            var expectedResponse = new UpdatedIngredientAliasResponse { Id = aliasId, IngredientId = ingredientId, AliasName = "Domatesik" };

            // Mock GetAsync to return the existing alias
            _mockAliasRepository
                .Setup(r => r.GetAsync(
                    It.Is<Expression<Func<IngredientAlias, bool>>>(expr => expr.Compile().Invoke(existingAlias)),
                    It.IsAny<Func<IQueryable<IngredientAlias>, IIncludableQueryable<IngredientAlias, object>>?>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingAlias);

            // Mock AliasNameCannotBeDuplicated - Neither Alias nor Ingredient with same name
            _mockAliasRepository
                .Setup(r => r.GetAsync(
                    It.Is<Expression<Func<IngredientAlias, bool>>>(expr => expr.Compile().Invoke(new IngredientAlias { AliasName = "Domatesik" })),
                    It.IsAny<Func<IQueryable<IngredientAlias>, IIncludableQueryable<IngredientAlias, object>>?>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((IngredientAlias)null!);

            _mockIngredientRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Ingredient, bool>>>(),
                    It.IsAny<Func<IQueryable<Ingredient>, IIncludableQueryable<Ingredient, object>>?>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Ingredient)null!);

            // Define mapping and update
            _mockMapper.Setup(m => m.Map(request, existingAlias)).Returns(mappedAlias);
            _mockAliasRepository.Setup(r => r.UpdateAsync(mappedAlias, It.IsAny<CancellationToken>())).ReturnsAsync(mappedAlias);
            _mockMapper.Setup(m => m.Map<UpdatedIngredientAliasResponse>(mappedAlias)).Returns(expectedResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.AliasName.Should().Be("Domatesik");
            result.Message.Should().Be(IngredientAliasMessages.AliasUpdatedMessage);

            _mockAliasRepository.Verify(r => r.UpdateAsync(mappedAlias, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
