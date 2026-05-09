using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.AllergyRestrictions.Commands.UpdateAllergyRestriction;
using OleAI.Backend.Application.Features.AllergyRestrictions.Constants;
using OleAI.Backend.Application.Features.AllergyRestrictions.Rules;
using OleAI.Backend.Application.Features.IngredientCategories.Rules;
using OleAI.Backend.Application.Features.Ingredients.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.AllergyRestrictions.Commands.UpdateAllergyRestriction;

public class UpdateAllergyRestrictionCommandHandlerTests
{
    private readonly Mock<IAllergyRestrictionRepository> _mockRepository;
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly AllergyRestrictionBusinessRules _businessRules;
    private readonly IngredientBusinessRules _ingredientRules;
    private readonly CategoryBusinessRules _categoryRules;
    private readonly UpdateAllergyRestrictionCommandHandler _handler;

    public UpdateAllergyRestrictionCommandHandlerTests()
    {
        _mockRepository = new Mock<IAllergyRestrictionRepository>();
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();

        _businessRules = new AllergyRestrictionBusinessRules(_mockRepository.Object);
        _ingredientRules = new IngredientBusinessRules(_mockIngredientRepository.Object);
        _categoryRules = new CategoryBusinessRules(_mockCategoryRepository.Object, _mockIngredientRepository.Object);

        _handler = new UpdateAllergyRestrictionCommandHandler(
            _mockRepository.Object,
            _businessRules,
            _ingredientRules,
            _categoryRules);
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_ShouldUpdateAndReturnSuccessMessage()
    {
        // Arrange
        var command = new UpdateAllergyRestrictionCommand
        {
            Id = Guid.NewGuid(),
            Name = "Gluten Free Updated",
            ExcludedIngredientIds = new List<Guid>(),
            ExcludedCategoryIds = new List<Guid>()
        };

        var restriction = new AllergyRestriction { Id = command.Id, Name = "Gluten Free" };

        _mockRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<AllergyRestriction, bool>>>(), It.IsAny<Func<IQueryable<AllergyRestriction>, IIncludableQueryable<AllergyRestriction, object>>>(), false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(restriction);

        _mockRepository
            .Setup(r => r.AnyAsync(It.IsAny<Expression<Func<AllergyRestriction, bool>>>(), false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<AllergyRestriction>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AllergyRestriction ar, CancellationToken _) => ar);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(AllergyRestrictionMessages.AllergyRestrictionUpdatedSuccessfully);
        _mockRepository.Verify(r => r.UpdateAsync(It.Is<AllergyRestriction>(ar => ar.Name == "Gluten Free Updated"), It.IsAny<CancellationToken>()), Times.Once);
    }
}
