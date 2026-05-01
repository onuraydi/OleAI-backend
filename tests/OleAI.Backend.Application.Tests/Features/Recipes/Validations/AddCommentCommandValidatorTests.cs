using FluentValidation.TestHelper;
using OleAI.Backend.Application.Features.Recipes.Commands.AddComment;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Validations;

public class AddCommentCommandValidatorTests
{
    private readonly AddCommentCommandValidator _validator;

    public AddCommentCommandValidatorTests()
    {
        _validator = new AddCommentCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Content_Is_Empty()
    {
        var command = new AddCommentCommand { Content = "" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Content);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Content_Is_Valid()
    {
        var command = new AddCommentCommand { Content = "This is a valid comment" };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
