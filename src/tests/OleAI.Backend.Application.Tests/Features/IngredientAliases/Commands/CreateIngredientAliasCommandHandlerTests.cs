using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.IngredientAliases.Commands.CreateIngredientAlias;
using OleAI.Backend.Application.Features.IngredientAliases.Constants;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Requests;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Responses;
using OleAI.Backend.Application.Features.IngredientAliases.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using System.Linq.Expressions;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;

namespace OleAI.Backend.Application.Tests.Features.IngredientAliases.Commands
{
    public class CreateIngredientAliasCommandHandlerTests
    {
        private readonly Mock<IIngredientAliasRepository> _mockAliasRepository;
        private readonly Mock<IIngredientRepository> _mockIngredientRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly IngredientAliasBusinessRules _businessRules;
        private readonly CreateIngredientAliasCommandHandler _handler;

        public CreateIngredientAliasCommandHandlerTests()
        {
            _mockAliasRepository = new Mock<IIngredientAliasRepository>();
            _mockIngredientRepository = new Mock<IIngredientRepository>();
            _mockMapper = new Mock<IMapper>();
            _businessRules = new IngredientAliasBusinessRules(_mockAliasRepository.Object, _mockIngredientRepository.Object);

            _handler = new CreateIngredientAliasCommandHandler(
                _mockAliasRepository.Object,
                _mockMapper.Object,
                _businessRules);
        }

        [Fact]
        public async Task Handle_WhenValidRequest_ShouldCreateAliasAndReturnResponse()
        {
            // Arrange
            var ingredientId = Guid.NewGuid();
            var request = new CreateIngredientAliasRequest { IngredientId = ingredientId, AliasName = "Domatesik" };
            var command = new CreateIngredientAliasCommand { Request = request };

            var mappedAlias = new IngredientAlias { IngredientId = ingredientId, AliasName = "Domatesik" };
            var createdAlias = new IngredientAlias { Id = Guid.NewGuid(), IngredientId = ingredientId, AliasName = "Domatesik" };
            var expectedResponse = new CreatedIngredientAliasResponse { IngredientId = ingredientId, AliasName = "Domatesik" };

            // Mock MasterIngredientMustExist
            _mockIngredientRepository
                .Setup(r => r.GetAsync(
                    It.Is<Expression<Func<Ingredient, bool>>>(expr => expr.Compile().Invoke(new Ingredient { Id = ingredientId })),
                    It.IsAny<Func<IQueryable<Ingredient>, IIncludableQueryable<Ingredient, object>>?>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Ingredient { Id = ingredientId });

            // Mock AliasNameCannotBeDuplicated - Neither Alias nor Ingredient with same name
            _mockAliasRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<IngredientAlias, bool>>>(),
                    It.IsAny<Func<IQueryable<IngredientAlias>, IIncludableQueryable<IngredientAlias, object>>?>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((IngredientAlias)null!);

            _mockIngredientRepository
                .Setup(r => r.GetAsync(
                    It.Is<Expression<Func<Ingredient, bool>>>(expr => expr.Compile().Invoke(new Ingredient { Name = "Domates" })), // Will mock default return
                    It.IsAny<Func<IQueryable<Ingredient>, IIncludableQueryable<Ingredient, object>>?>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Ingredient)null!);

            _mockMapper.Setup(m => m.Map<IngredientAlias>(request)).Returns(mappedAlias);
            _mockAliasRepository.Setup(r => r.AddAsync(mappedAlias, It.IsAny<CancellationToken>())).ReturnsAsync(createdAlias);
            _mockMapper.Setup(m => m.Map<CreatedIngredientAliasResponse>(createdAlias)).Returns(expectedResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IngredientId.Should().Be(ingredientId);
            result.AliasName.Should().Be("Domatesik");
            result.Message.Should().Be(IngredientAliasMessages.AliasCreatedMessage);

            _mockAliasRepository.Verify(r => r.AddAsync(mappedAlias, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
