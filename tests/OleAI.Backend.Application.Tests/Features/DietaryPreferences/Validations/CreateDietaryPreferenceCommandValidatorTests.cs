using FluentValidation.TestHelper;
using OleAI.Backend.Application.Features.DietaryPreferences.Commands.CreateDietaryPreference;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.DietaryPreferences.Validations;

public class CreateDietaryPreferenceCommandValidatorTests
{
    private readonly CreateDietaryPreferenceCommandValidator _validator;

    public CreateDietaryPreferenceCommandValidatorTests()
    {
        _validator = new CreateDietaryPreferenceCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var command = new CreateDietaryPreferenceCommand { Name = string.Empty };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Name_Is_Specified()
    {
        var command = new CreateDietaryPreferenceCommand { Name = "Vegan" };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_ExcludedIngredientIds_Is_Null()
    {
        var command = new CreateDietaryPreferenceCommand { ExcludedIngredientIds = null! };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ExcludedIngredientIds);
    }

    [Fact]
    public void Should_Have_Error_When_ExcludedCategoryIds_Is_Null()
    {
        var command = new CreateDietaryPreferenceCommand { ExcludedCategoryIds = null! };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ExcludedCategoryIds);
    }
}
