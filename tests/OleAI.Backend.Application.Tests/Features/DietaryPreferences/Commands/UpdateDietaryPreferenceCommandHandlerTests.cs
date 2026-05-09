using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.DietaryPreferences.Commands.UpdateDietaryPreference;
using OleAI.Backend.Application.Features.DietaryPreferences.Constants;
using OleAI.Backend.Application.Features.DietaryPreferences.Rules;
using OleAI.Backend.Application.Features.IngredientCategories.Rules;
using OleAI.Backend.Application.Features.Ingredients.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.DietaryPreferences.Commands.UpdateDietaryPreference;

public class UpdateDietaryPreferenceCommandHandlerTests
{
    private readonly Mock<IDietaryPreferenceRepository> _mockDietaryPreferenceRepository;
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly DietaryPreferenceBusinessRules _businessRules;
    private readonly IngredientBusinessRules _ingredientRules;
    private readonly CategoryBusinessRules _categoryRules;
    private readonly UpdateDietaryPreferenceCommandHandler _handler;

    public UpdateDietaryPreferenceCommandHandlerTests()
    {
        _mockDietaryPreferenceRepository = new Mock<IDietaryPreferenceRepository>();
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();

        _businessRules = new DietaryPreferenceBusinessRules(_mockDietaryPreferenceRepository.Object);
        _ingredientRules = new IngredientBusinessRules(_mockIngredientRepository.Object);
        _categoryRules = new CategoryBusinessRules(_mockCategoryRepository.Object, _mockIngredientRepository.Object);

        _handler = new UpdateDietaryPreferenceCommandHandler(
            _mockDietaryPreferenceRepository.Object,
            _businessRules,
            _ingredientRules,
            _categoryRules);
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_ShouldUpdateAndReturnSuccessMessage()
    {
        // Arrange
        var command = new UpdateDietaryPreferenceCommand
        {
            Name = "Yeni Vegan",
            Description = "Yeni açıklama",
            ExcludedIngredientIds = new List<Guid> { Guid.NewGuid() },
            ExcludedCategoryIds = new List<Guid> { Guid.NewGuid() }
        };

        var preference = new DietaryPreference { Id = command.Id, Name = "Vegan" };

        _mockDietaryPreferenceRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<DietaryPreference, bool>>>(), It.IsAny<Func<IQueryable<DietaryPreference>, IIncludableQueryable<DietaryPreference, object>>>(), false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        _mockDietaryPreferenceRepository
            .Setup(r => r.AnyAsync(It.IsAny<Expression<Func<DietaryPreference, bool>>>(), false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockDietaryPreferenceRepository
            .Setup(r => r.UpdateAsync(It.IsAny<DietaryPreference>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((DietaryPreference dp, CancellationToken _) => dp);

        _mockIngredientRepository
            .Setup(r => r.GetCountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Ingredient, bool>>>(), false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(command.ExcludedIngredientIds.Count);

        _mockCategoryRepository
            .Setup(r => r.GetCountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>(), false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(command.ExcludedCategoryIds.Count);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(DietaryPreferenceMessages.DietaryPreferenceUpdatedSuccessfully);
        _mockDietaryPreferenceRepository.Verify(r => r.UpdateAsync(It.Is<DietaryPreference>(dp => dp.Name == "Yeni Vegan"), It.IsAny<CancellationToken>()), Times.Once);
    }
}
