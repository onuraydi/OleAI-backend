using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.IngredientAliases.Commands.DeleteIngredientAlias;
using OleAI.Backend.Application.Features.IngredientAliases.Constants;
using OleAI.Backend.Application.Features.IngredientAliases.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using System.Linq.Expressions;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;

namespace OleAI.Backend.Application.Tests.Features.IngredientAliases.Commands
{
    public class DeleteIngredientAliasCommandHandlerTests
    {
        private readonly Mock<IIngredientAliasRepository> _mockAliasRepository;
        private readonly Mock<IIngredientRepository> _mockIngredientRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly IngredientAliasBusinessRules _businessRules;
        private readonly DeleteIngredientAliasCommandHandler _handler;

        public DeleteIngredientAliasCommandHandlerTests()
        {
            _mockAliasRepository = new Mock<IIngredientAliasRepository>();
            _mockIngredientRepository = new Mock<IIngredientRepository>();
            _mockMapper = new Mock<IMapper>();
            _businessRules = new IngredientAliasBusinessRules(_mockAliasRepository.Object, _mockIngredientRepository.Object);

            _handler = new DeleteIngredientAliasCommandHandler(
                _mockAliasRepository.Object,
                _mockMapper.Object,
                _businessRules);
        }

        [Fact]
        public async Task Handle_WhenValidRequest_ShouldDeleteAliasAndReturnMessage()
        {
            // Arrange
            var aliasId = Guid.NewGuid();
            var command = new DeleteIngredientAliasCommand { Id = aliasId };
            var existingAlias = new IngredientAlias { Id = aliasId, AliasName = "Domat" };

            // Mock GetAsync to return the existing alias
            _mockAliasRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<IngredientAlias, bool>>>(),
                    null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingAlias);

            _mockAliasRepository.Setup(r => r.DeleteAsync(existingAlias, false, It.IsAny<CancellationToken>())).ReturnsAsync(existingAlias);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(IngredientAliasMessages.AliasDeletedMessage);
            _mockAliasRepository.Verify(r => r.DeleteAsync(existingAlias, false, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenAliasNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var command = new DeleteIngredientAliasCommand { Id = Guid.NewGuid() };

            _mockAliasRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<IngredientAlias, bool>>>(),
                    null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync((IngredientAlias)null!);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage(IngredientAliasMessages.AliasNotFoundMessage);

            _mockAliasRepository.Verify(r => r.DeleteAsync(It.IsAny<IngredientAlias>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
