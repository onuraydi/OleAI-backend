using FluentValidation.TestHelper;
using OleAI.Backend.Application.Features.DietaryPreferences.Commands.UpdateDietaryPreference;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.DietaryPreferences.Validations;

public class UpdateDietaryPreferenceCommandValidatorTests
{
    private readonly UpdateDietaryPreferenceCommandValidator _validator;

    public UpdateDietaryPreferenceCommandValidatorTests()
    {
        _validator = new UpdateDietaryPreferenceCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Id_Is_Empty()
    {
        var command = new UpdateDietaryPreferenceCommand { Id = Guid.Empty };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var command = new UpdateDietaryPreferenceCommand { Name = string.Empty };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Fields_Are_Valid()
    {
        var command = new UpdateDietaryPreferenceCommand { Id = Guid.NewGuid(), Name = "Vegan" };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}
