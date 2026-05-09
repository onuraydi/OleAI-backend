using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.AllergyRestrictions.Commands.CreateAllergyRestriction;
using OleAI.Backend.Application.Features.AllergyRestrictions.Constants;
using OleAI.Backend.Application.Features.AllergyRestrictions.Rules;
using OleAI.Backend.Application.Features.IngredientCategories.Rules;
using OleAI.Backend.Application.Features.Ingredients.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.AllergyRestrictions.Commands.CreateAllergyRestriction;

public class CreateAllergyRestrictionCommandHandlerTests
{
    private readonly Mock<IAllergyRestrictionRepository> _mockRepository;
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly AllergyRestrictionBusinessRules _businessRules;
    private readonly IngredientBusinessRules _ingredientRules;
    private readonly CategoryBusinessRules _categoryRules;
    private readonly CreateAllergyRestrictionCommandHandler _handler;

    public CreateAllergyRestrictionCommandHandlerTests()
    {
        _mockRepository = new Mock<IAllergyRestrictionRepository>();
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();

        _businessRules = new AllergyRestrictionBusinessRules(_mockRepository.Object);
        _ingredientRules = new IngredientBusinessRules(_mockIngredientRepository.Object);
        _categoryRules = new CategoryBusinessRules(_mockCategoryRepository.Object, _mockIngredientRepository.Object);

        _handler = new CreateAllergyRestrictionCommandHandler(
            _mockRepository.Object,
            _businessRules,
            _ingredientRules,
            _categoryRules);
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_ShouldCreateAndReturnSuccessMessage()
    {
        // Arrange
        var command = new CreateAllergyRestrictionCommand
        {
            Name = "Gluten Allergy",
            Description = "No gluten",
            ExcludedIngredientIds = new List<Guid>(),
            ExcludedCategoryIds = new List<Guid>()
        };

        _mockRepository
            .Setup(r => r.AnyAsync(dp => dp.Name.ToLower() == command.Name.ToLower(), false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<AllergyRestriction>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AllergyRestriction ar, CancellationToken _) => ar);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(AllergyRestrictionMessages.AllergyRestrictionCreatedSuccessfully);
        _mockRepository.Verify(r => r.AddAsync(It.Is<AllergyRestriction>(ar => ar.Name == "Gluten Allergy"), It.IsAny<CancellationToken>()), Times.Once);
    }
}
