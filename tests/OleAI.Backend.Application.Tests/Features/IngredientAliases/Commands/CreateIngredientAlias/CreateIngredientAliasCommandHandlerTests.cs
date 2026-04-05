using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.IngredientAliases.Commands.CreateIngredientAlias;
using OleAI.Backend.Application.Features.IngredientAliases.Constants;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Requests;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Responses;
using OleAI.Backend.Application.Features.IngredientAliases.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.IngredientAliases.Commands.CreateIngredientAlias;

public class CreateIngredientAliasCommandHandlerTests
{
    private readonly Mock<IIngredientAliasRepository> _mockAliasRepository;
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly IngredientAliasBusinessRules _rules;
    private readonly CreateIngredientAliasCommandHandler _handler;

    public CreateIngredientAliasCommandHandlerTests()
    {
        _mockAliasRepository = new Mock<IIngredientAliasRepository>();
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockMapper = new Mock<IMapper>();

        _rules = new IngredientAliasBusinessRules(_mockAliasRepository.Object, _mockIngredientRepository.Object);
        _handler = new CreateIngredientAliasCommandHandler(_mockAliasRepository.Object, _mockMapper.Object, _rules);
    }

    [Fact]
    public async Task Handle_WhenMasterIngredientMissing_ShouldThrowBusinessException()
    {
        var command = new CreateIngredientAliasCommand
        {
            Request = new CreateIngredientAliasRequest { IngredientId = Guid.NewGuid(), AliasName = "Domat" }
        };

        _mockIngredientRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ingredient?)null);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage(IngredientAliasMessages.MasterMaterialNotFoundMessage);
    }

    [Fact]
    public async Task Handle_WhenRequestValid_ShouldCreateAliasAndReturnResponse()
    {
        var ingredientId = Guid.NewGuid();
        var command = new CreateIngredientAliasCommand
        {
            Request = new CreateIngredientAliasRequest { IngredientId = ingredientId, AliasName = "Domat" }
        };

        var mappedAlias = new IngredientAlias(Guid.NewGuid(), ingredientId, "Domat");
        var mappedResponse = new CreatedIngredientAliasResponse { IngredientId = ingredientId, AliasName = "Domat" };

        _mockIngredientRepository
            .SetupSequence(r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Ingredient(ingredientId, "Domates", Guid.NewGuid(), "adet"))
            .ReturnsAsync((Ingredient?)null);

        _mockAliasRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<IngredientAlias, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IngredientAlias?)null);

        _mockMapper.Setup(m => m.Map<IngredientAlias>(command.Request)).Returns(mappedAlias);
        _mockAliasRepository.Setup(r => r.AddAsync(mappedAlias, It.IsAny<CancellationToken>())).ReturnsAsync(mappedAlias);
        _mockMapper.Setup(m => m.Map<CreatedIngredientAliasResponse>(mappedAlias)).Returns(mappedResponse);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IngredientId.Should().Be(ingredientId);
        result.AliasName.Should().Be("Domat");
        result.Message.Should().Be(IngredientAliasMessages.AliasCreatedMessage);
    }
}
