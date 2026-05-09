using FluentValidation.TestHelper;
using OleAI.Backend.Application.Features.AllergyRestrictions.Commands.CreateAllergyRestriction;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.AllergyRestrictions.Validations;

public class CreateAllergyRestrictionCommandValidatorTests
{
    private readonly CreateAllergyRestrictionCommandValidator _validator;

    public CreateAllergyRestrictionCommandValidatorTests()
    {
        _validator = new CreateAllergyRestrictionCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var command = new CreateAllergyRestrictionCommand { Name = string.Empty };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Name_Is_Specified()
    {
        var command = new CreateAllergyRestrictionCommand { Name = "Glutensiz" };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_ExcludedIngredientIds_Is_Null()
    {
        var command = new CreateAllergyRestrictionCommand { ExcludedIngredientIds = null! };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ExcludedIngredientIds);
    }

    [Fact]
    public void Should_Have_Error_When_ExcludedCategoryIds_Is_Null()
    {
        var command = new CreateAllergyRestrictionCommand { ExcludedCategoryIds = null! };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ExcludedCategoryIds);
    }
}
