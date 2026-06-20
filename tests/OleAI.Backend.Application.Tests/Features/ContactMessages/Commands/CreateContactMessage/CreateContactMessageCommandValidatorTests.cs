using FluentValidation.TestHelper;
using OleAI.Backend.Application.Features.ContactMessages.Commands.CreateContactMessage;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.ContactMessages.Commands.CreateContactMessage;

public class CreateContactMessageCommandValidatorTests
{
    private readonly CreateContactMessageCommandValidator _validator;

    public CreateContactMessageCommandValidatorTests()
    {
        _validator = new CreateContactMessageCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var command = new CreateContactMessageCommand { Name = "", Email = "test@test.com", Department = "Tech", Message = "Long enough message" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Model_Is_Valid()
    {
        var command = new CreateContactMessageCommand { Name = "John", Email = "test@test.com", Department = "Tech", Message = "Long enough message" };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(c => c.Name);
        result.ShouldNotHaveValidationErrorFor(c => c.Email);
        result.ShouldNotHaveValidationErrorFor(c => c.Department);
        result.ShouldNotHaveValidationErrorFor(c => c.Message);
    }
}
