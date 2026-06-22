using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.IngredientAliases.Commands.DeleteIngredientAlias;
using OleAI.Backend.Application.Features.IngredientAliases.Constants;
using OleAI.Backend.Application.Features.IngredientAliases.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.IngredientAliases.Commands.DeleteIngredientAlias;

public class DeleteIngredientAliasCommandHandlerTests
{
    private readonly Mock<IIngredientAliasRepository> _mockAliasRepository;
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly IngredientAliasBusinessRules _rules;
    private readonly DeleteIngredientAliasCommandHandler _handler;

    public DeleteIngredientAliasCommandHandlerTests()
    {
        _mockAliasRepository = new Mock<IIngredientAliasRepository>();
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockCacheService = new Mock<ICacheService>();

        _rules = new IngredientAliasBusinessRules(_mockAliasRepository.Object, _mockIngredientRepository.Object);
        _handler = new DeleteIngredientAliasCommandHandler(_mockAliasRepository.Object, _mockMapper.Object, _rules, _mockCacheService.Object);
    }

    [Fact]
    public async Task Handle_WhenAliasNotFound_ShouldThrowNotFoundException()
    {
        var command = new DeleteIngredientAliasCommand { Id = Guid.NewGuid() };

        _mockAliasRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<IngredientAlias, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IngredientAlias?)null);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(IngredientAliasMessages.AliasNotFoundMessage);
    }

    [Fact]
    public async Task Handle_WhenAliasExists_ShouldDeleteAndReturnMessage()
    {
        var alias = new IngredientAlias(Guid.NewGuid(), Guid.NewGuid(), "Domat");
        var command = new DeleteIngredientAliasCommand { Id = alias.Id };

        _mockAliasRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<IngredientAlias, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(alias);

        _mockAliasRepository
            .Setup(r => r.DeleteAsync(alias, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(alias);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(IngredientAliasMessages.AliasDeletedMessage);
        _mockAliasRepository.Verify(r => r.DeleteAsync(alias, false, It.IsAny<CancellationToken>()), Times.Once);
    }
}
