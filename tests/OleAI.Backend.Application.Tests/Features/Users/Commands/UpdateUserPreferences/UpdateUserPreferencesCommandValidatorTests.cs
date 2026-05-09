using FluentAssertions;
using FluentValidation.TestHelper;
using OleAI.Backend.Application.Features.Users.Commands.UpdateUserPreferences;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Users.Commands.UpdateUserPreferences;

public class UpdateUserPreferencesCommandValidatorTests
{
    private readonly UpdateUserPreferencesCommandValidator _validator;

    public UpdateUserPreferencesCommandValidatorTests()
    {
        _validator = new UpdateUserPreferencesCommandValidator();
    }

    [Fact]
    public void Validate_WhenListIsNull_ShouldHaveError()
    {
        var command = new UpdateUserPreferencesCommand { DietaryPreferenceIds = null! };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.DietaryPreferenceIds);
    }

    [Fact]
    public void Validate_WhenListContainsEmptyGuid_ShouldHaveError()
    {
        var command = new UpdateUserPreferencesCommand { DietaryPreferenceIds = new List<Guid> { Guid.Empty } };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor("DietaryPreferenceIds[0]");
    }

    [Fact]
    public void Validate_WhenEmptyList_ShouldNotHaveErrors()
    {
        var command = new UpdateUserPreferencesCommand { DietaryPreferenceIds = new List<Guid>() };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenValidInput_ShouldNotHaveErrors()
    {
        var command = new UpdateUserPreferencesCommand
        {
            DietaryPreferenceIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
