using System.Linq.Expressions;
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
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.IngredientAliases.Commands.UpdateIngredientAlias;

public class UpdateIngredientAliasCommandHandlerTests
{
    private readonly Mock<IIngredientAliasRepository> _mockAliasRepository;
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly IngredientAliasBusinessRules _rules;
    private readonly UpdateIngredientAliasCommandHandler _handler;

    public UpdateIngredientAliasCommandHandlerTests()
    {
        _mockAliasRepository = new Mock<IIngredientAliasRepository>();
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockMapper = new Mock<IMapper>();

        _rules = new IngredientAliasBusinessRules(_mockAliasRepository.Object, _mockIngredientRepository.Object);
        _handler = new UpdateIngredientAliasCommandHandler(_mockAliasRepository.Object, _mockMapper.Object, _rules);
    }

    [Fact]
    public async Task Handle_WhenAliasNotFound_ShouldThrowNotFoundException()
    {
        var command = new UpdateIngredientAliasCommand
        {
            Request = new UpdateIngredientAliasRequest { Id = Guid.NewGuid(), IngredientId = Guid.NewGuid(), AliasName = "Domat" }
        };

        _mockAliasRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<IngredientAlias, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IngredientAlias?)null);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(IngredientAliasMessages.AliasNotFoundMessage);
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldUpdateAliasAndReturnResponse()
    {
        var alias = new IngredientAlias(Guid.NewGuid(), Guid.NewGuid(), "Domat");
        var command = new UpdateIngredientAliasCommand
        {
            Request = new UpdateIngredientAliasRequest { Id = alias.Id, IngredientId = alias.IngredientId, AliasName = "Domatescik" }
        };

        var mappedAlias = alias;
        var mappedResponse = new UpdatedIngredientAliasResponse { IngredientId = alias.IngredientId, AliasName = "Domatescik" };

        _mockAliasRepository
            .SetupSequence(r => r.GetAsync(It.IsAny<Expression<Func<IngredientAlias, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(alias)
            .ReturnsAsync((IngredientAlias?)null);

        _mockIngredientRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ingredient?)null);

        _mockMapper.Setup(m => m.Map(command.Request, alias)).Returns(mappedAlias);
        _mockAliasRepository.Setup(r => r.UpdateAsync(mappedAlias, It.IsAny<CancellationToken>())).ReturnsAsync(mappedAlias);
        _mockMapper.Setup(m => m.Map<UpdatedIngredientAliasResponse>(mappedAlias)).Returns(mappedResponse);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.AliasName.Should().Be("Domatescik");
        result.Message.Should().Be(IngredientAliasMessages.AliasUpdatedMessage);

        _mockAliasRepository.Verify(r => r.UpdateAsync(mappedAlias, It.IsAny<CancellationToken>()), Times.Once);
    }
}
