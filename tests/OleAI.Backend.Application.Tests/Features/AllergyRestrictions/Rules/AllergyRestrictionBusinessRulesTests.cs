using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.AllergyRestrictions.Constants;
using OleAI.Backend.Application.Features.AllergyRestrictions.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.AllergyRestrictions.Rules;

public class AllergyRestrictionBusinessRulesTests
{
    private readonly Mock<IAllergyRestrictionRepository> _mockRepository;
    private readonly AllergyRestrictionBusinessRules _rules;

    public AllergyRestrictionBusinessRulesTests()
    {
        _mockRepository = new Mock<IAllergyRestrictionRepository>();
        _rules = new AllergyRestrictionBusinessRules(_mockRepository.Object);
    }

    [Fact]
    public async Task CheckIfAllergyRestrictionNameExistsAsync_ThrowsBusinessException_WhenNameExists()
    {
        _mockRepository.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<AllergyRestriction, bool>>>(), false, true, default))
            .ReturnsAsync(true);

        Func<Task> action = async () => await _rules.CheckIfAllergyRestrictionNameExistsAsync("Glutensiz");
        await action.Should().ThrowAsync<BusinessException>().WithMessage(AllergyRestrictionMessages.AllergyRestrictionNameAlreadyExists);
    }

    [Fact]
    public async Task CheckIfAllergyRestrictionNameExistsWhenUpdatingAsync_ThrowsBusinessException_WhenNameExists()
    {
        _mockRepository.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<AllergyRestriction, bool>>>(), false, true, default))
            .ReturnsAsync(true);

        Func<Task> action = async () => await _rules.CheckIfAllergyRestrictionNameExistsWhenUpdatingAsync("Glutensiz", Guid.NewGuid());
        await action.Should().ThrowAsync<BusinessException>().WithMessage(AllergyRestrictionMessages.AllergyRestrictionNameAlreadyExists);
    }

    [Fact]
    public void CheckIfAllergyRestrictionExists_ThrowsNotFoundException_WhenNull()
    {
        Action action = () => _rules.CheckIfAllergyRestrictionExists(null);
        action.Should().Throw<NotFoundException>().WithMessage(AllergyRestrictionMessages.AllergyRestrictionNotFound);
    }

    [Fact]
    public void CheckIfAllergyRestrictionExists_DoesNotThrow_WhenNotNull()
    {
        var ar = new AllergyRestriction();
        Action action = () => _rules.CheckIfAllergyRestrictionExists(ar);
        action.Should().NotThrow();
    }
}
