using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.DietaryPreferences.Commands.CreateDietaryPreference;
using OleAI.Backend.Application.Features.DietaryPreferences.Constants;
using OleAI.Backend.Application.Features.DietaryPreferences.Rules;
using OleAI.Backend.Application.Features.IngredientCategories.Rules;
using OleAI.Backend.Application.Features.Ingredients.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.DietaryPreferences.Commands.CreateDietaryPreference;

public class CreateDietaryPreferenceCommandHandlerTests
{
    private readonly Mock<IDietaryPreferenceRepository> _mockDietaryPreferenceRepository;
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly DietaryPreferenceBusinessRules _businessRules;
    private readonly IngredientBusinessRules _ingredientRules;
    private readonly CategoryBusinessRules _categoryRules;
    private readonly CreateDietaryPreferenceCommandHandler _handler;

    public CreateDietaryPreferenceCommandHandlerTests()
    {
        _mockDietaryPreferenceRepository = new Mock<IDietaryPreferenceRepository>();
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();

        _businessRules = new DietaryPreferenceBusinessRules(_mockDietaryPreferenceRepository.Object);
        _ingredientRules = new IngredientBusinessRules(_mockIngredientRepository.Object);
        _categoryRules = new CategoryBusinessRules(_mockCategoryRepository.Object, _mockIngredientRepository.Object);

        _handler = new CreateDietaryPreferenceCommandHandler(
            _mockDietaryPreferenceRepository.Object,
            _businessRules,
            _ingredientRules,
            _categoryRules);
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_ShouldCreateAndReturnSuccessMessage()
    {
        // Arrange
        var command = new CreateDietaryPreferenceCommand
        {
            Name = "Vegan",
            Description = "Hayvansal gıdalar tüketilmez",
            ExcludedIngredientIds = new List<Guid> { Guid.NewGuid() },
            ExcludedCategoryIds = new List<Guid> { Guid.NewGuid() }
        };

        _mockDietaryPreferenceRepository
            .Setup(r => r.AnyAsync(dp => dp.Name.ToLower() == command.Name.ToLower(), false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockDietaryPreferenceRepository
            .Setup(r => r.AddAsync(It.IsAny<DietaryPreference>(), It.IsAny<CancellationToken>()))
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
        result.Should().Be(DietaryPreferenceMessages.DietaryPreferenceCreatedSuccessfully);
        _mockDietaryPreferenceRepository.Verify(r => r.AddAsync(It.Is<DietaryPreference>(dp => dp.Name == "Vegan"), It.IsAny<CancellationToken>()), Times.Once);
    }
}
