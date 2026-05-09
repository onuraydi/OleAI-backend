using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.DietaryPreferences.Constants;
using OleAI.Backend.Application.Features.DietaryPreferences.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.DietaryPreferences.Rules;

public class DietaryPreferenceBusinessRulesTests
{
    private readonly Mock<IDietaryPreferenceRepository> _mockRepository;
    private readonly DietaryPreferenceBusinessRules _rules;

    public DietaryPreferenceBusinessRulesTests()
    {
        _mockRepository = new Mock<IDietaryPreferenceRepository>();
        _rules = new DietaryPreferenceBusinessRules(_mockRepository.Object);
    }

    [Fact]
    public async Task CheckIfDietaryPreferenceNameExistsAsync_ThrowsBusinessException_WhenNameExists()
    {
        _mockRepository.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<DietaryPreference, bool>>>(), false, true, default))
            .ReturnsAsync(true);

        Func<Task> action = async () => await _rules.CheckIfDietaryPreferenceNameExistsAsync("Vegan");
        await action.Should().ThrowAsync<BusinessException>().WithMessage(DietaryPreferenceMessages.DietaryPreferenceNameAlreadyExists);
    }

    [Fact]
    public async Task CheckIfDietaryPreferenceNameExistsWhenUpdatingAsync_ThrowsBusinessException_WhenNameExists()
    {
        _mockRepository.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<DietaryPreference, bool>>>(), false, true, default))
            .ReturnsAsync(true);

        Func<Task> action = async () => await _rules.CheckIfDietaryPreferenceNameExistsWhenUpdatingAsync("Vegan", Guid.NewGuid());
        await action.Should().ThrowAsync<BusinessException>().WithMessage(DietaryPreferenceMessages.DietaryPreferenceNameAlreadyExists);
    }

    [Fact]
    public void CheckIfDietaryPreferenceExists_ThrowsNotFoundException_WhenNull()
    {
        Action action = () => _rules.CheckIfDietaryPreferenceExists(null);
        action.Should().Throw<NotFoundException>().WithMessage(DietaryPreferenceMessages.DietaryPreferenceNotFound);
    }

    [Fact]
    public void CheckIfDietaryPreferenceExists_DoesNotThrow_WhenNotNull()
    {
        var diet = new DietaryPreference();
        Action action = () => _rules.CheckIfDietaryPreferenceExists(diet);
        action.Should().NotThrow();
    }
}
