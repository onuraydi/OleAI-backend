using FluentValidation.TestHelper;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Commands.CreateRecipe;
using OleAI.Backend.Application.Features.Recipes.Dtos.Requests;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Validations;

public class CreateRecipeCommandValidatorTests
{
    private readonly CreateRecipeCommandValidator _validator;

    public CreateRecipeCommandValidatorTests()
    {
        _validator = new CreateRecipeCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Empty()
    {
        var command = new CreateRecipeCommand { CreateRecipeRequest = new CreateRecipeRequest { Title = "" } };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.CreateRecipeRequest.Title);
    }

    [Fact]
    public void Should_Have_Error_When_Ingredients_Are_Empty()
    {
        var command = new CreateRecipeCommand
        {
            CreateRecipeRequest = new CreateRecipeRequest { Title = "Title" },
            Ingredients = new List<CreateRecipeIngredientRequest>()
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Ingredients);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = new CreateRecipeCommand
        {
            CreateRecipeRequest = new CreateRecipeRequest
            {
                Title = "Valid Title",
                MainImage = Mock.Of<Microsoft.AspNetCore.Http.IFormFile>(),
                Servings = 2,
                PrepTime = 10,
                CookTime = 20,
                Difficulty = RecipeDifficulty.Medium
            },
            Ingredients = new List<CreateRecipeIngredientRequest> { new() { IngredientId = Guid.NewGuid(), Amount = 10 } },
            Steps = new List<CreateRecipeStepRequest> { new() { StepNumber = 1, Instruction = "Step 1" } }
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
