using FluentAssertions;
using FluentValidation.TestHelper;
using OleAI.Backend.Application.Features.Users.Commands.UpdateUserAllergies;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Users.Commands.UpdateUserAllergies;

public class UpdateUserAllergiesCommandValidatorTests
{
    private readonly UpdateUserAllergiesCommandValidator _validator;

    public UpdateUserAllergiesCommandValidatorTests()
    {
        _validator = new UpdateUserAllergiesCommandValidator();
    }

    [Fact]
    public void Validate_WhenListIsNull_ShouldHaveError()
    {
        var command = new UpdateUserAllergiesCommand { AllergyRestrictionIds = null! };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.AllergyRestrictionIds);
    }

    [Fact]
    public void Validate_WhenListContainsEmptyGuid_ShouldHaveError()
    {
        var command = new UpdateUserAllergiesCommand { AllergyRestrictionIds = new List<Guid> { Guid.Empty } };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor("AllergyRestrictionIds[0]");
    }

    [Fact]
    public void Validate_WhenEmptyList_ShouldNotHaveErrors()
    {
        var command = new UpdateUserAllergiesCommand { AllergyRestrictionIds = new List<Guid>() };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenValidInput_ShouldNotHaveErrors()
    {
        var command = new UpdateUserAllergiesCommand
        {
            AllergyRestrictionIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
