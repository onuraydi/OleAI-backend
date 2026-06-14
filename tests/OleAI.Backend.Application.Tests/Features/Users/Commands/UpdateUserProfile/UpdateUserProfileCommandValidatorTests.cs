using FluentAssertions;
using FluentValidation.TestHelper;
using OleAI.Backend.Application.Features.Users.Commands.UpdateUserProfile;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Users.Commands.UpdateUserProfile;

public class UpdateUserProfileCommandValidatorTests
{
    private readonly UpdateUserProfileCommandValidator _validator;

    public UpdateUserProfileCommandValidatorTests()
    {
        _validator = new UpdateUserProfileCommandValidator();
    }


    [Fact]
    public void Validate_WhenFirstNameIsEmpty_ShouldNotHaveError()
    {
        var command = new UpdateUserProfileCommand { FirstName = "", LastName = "Test", Username = "valid_user" };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validate_WhenLastNameIsEmpty_ShouldNotHaveError()
    {
        var command = new UpdateUserProfileCommand { FirstName = "Test", LastName = "", Username = "valid_user" };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Validate_WhenFirstNameExceedsMaxLength_ShouldHaveError()
    {
        var command = new UpdateUserProfileCommand { FirstName = new string('A', 51), LastName = "Test", Username = "valid_user" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validate_WhenLastNameExceedsMaxLength_ShouldHaveError()
    {
        var command = new UpdateUserProfileCommand { FirstName = "Test", LastName = new string('A', 51), Username = "valid_user" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Validate_WhenValidInput_ShouldNotHaveErrors()
    {
        var command = new UpdateUserProfileCommand { FirstName = "John", LastName = "Doe", Username = "john_doe" };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
