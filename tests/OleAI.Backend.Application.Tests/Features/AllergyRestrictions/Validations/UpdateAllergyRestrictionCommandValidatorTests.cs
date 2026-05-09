using FluentValidation.TestHelper;
using OleAI.Backend.Application.Features.AllergyRestrictions.Commands.UpdateAllergyRestriction;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.AllergyRestrictions.Validations;

public class UpdateAllergyRestrictionCommandValidatorTests
{
    private readonly UpdateAllergyRestrictionCommandValidator _validator;

    public UpdateAllergyRestrictionCommandValidatorTests()
    {
        _validator = new UpdateAllergyRestrictionCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Id_Is_Empty()
    {
        var command = new UpdateAllergyRestrictionCommand { Id = Guid.Empty };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var command = new UpdateAllergyRestrictionCommand { Name = string.Empty };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Fields_Are_Valid()
    {
        var command = new UpdateAllergyRestrictionCommand { Id = Guid.NewGuid(), Name = "Glutensiz" };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}
