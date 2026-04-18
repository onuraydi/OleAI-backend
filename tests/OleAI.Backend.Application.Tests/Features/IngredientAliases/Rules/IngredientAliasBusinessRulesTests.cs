using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.IngredientAliases.Constants;
using OleAI.Backend.Application.Features.IngredientAliases.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.IngredientAliases.Rules;

public class IngredientAliasBusinessRulesTests
{
    private readonly Mock<IIngredientAliasRepository> _mockAliasRepository;
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly IngredientAliasBusinessRules _rules;

    public IngredientAliasBusinessRulesTests()
    {
        _mockAliasRepository = new Mock<IIngredientAliasRepository>();
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _rules = new IngredientAliasBusinessRules(_mockAliasRepository.Object, _mockIngredientRepository.Object);
    }

    [Fact]
    public void AliasMustExistWhenRequested_WhenAliasIsNull_ShouldThrowNotFoundException()
    {
        Action act = () => _rules.AliasMustExistWhenRequested(null);

        act.Should().Throw<NotFoundException>()
            .WithMessage(IngredientAliasMessages.AliasNotFoundMessage);
    }

    [Fact]
    public async Task MasterIngredientMustExist_WhenNotFound_ShouldThrowBusinessException()
    {
        _mockIngredientRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ingredient?)null);

        Func<Task> act = async () => await _rules.MasterIngredientMustExist(Guid.NewGuid());

        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage(IngredientAliasMessages.MasterMaterialNotFoundMessage);
    }

    [Fact]
    public async Task AliasNameCannotBeDuplicated_WhenAliasExists_ShouldThrowBusinessException()
    {
        _mockAliasRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<IngredientAlias, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IngredientAlias(Guid.NewGuid(), Guid.NewGuid(), "Domat"));

        Func<Task> act = async () => await _rules.AliasNameCannotBeDuplicated("Domat");

        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("'Domat' kelimesi zaten sistemde eşanlamlı olarak kayıtlı.");
    }

    [Fact]
    public async Task AliasNameCannotBeDuplicated_WhenMasterNameExists_ShouldThrowBusinessException()
    {
        _mockAliasRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<IngredientAlias, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IngredientAlias?)null);

        _mockIngredientRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Ingredient(Guid.NewGuid(), "Domat", Guid.NewGuid(), MeasurementCategory.Count));

        Func<Task> act = async () => await _rules.AliasNameCannotBeDuplicated("Domat");

        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("'Domat' kelimesi sistemde zaten bir Ana Malzeme olarak bulunuyor, eşanlamlı olarak eklenemez.");
    }
}
