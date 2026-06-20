using FluentValidation.TestHelper;
using OleAI.Backend.Application.Features.Recipes.Commands.UpdateRecipe;
using OleAI.Backend.Application.Features.Recipes.Dtos.Requests;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Commands.UpdateRecipe;

public class UpdateRecipeCommandValidatorTests
{
    private readonly UpdateRecipeCommandValidator _validator;

    public UpdateRecipeCommandValidatorTests()
    {
        _validator = new UpdateRecipeCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Empty()
    {
        var command = new UpdateRecipeCommand
        {
            Request = new UpdateRecipeRequest { Title = "" },
            Ingredients = new System.Collections.Generic.List<CreateRecipeIngredientRequest> { new CreateRecipeIngredientRequest { IngredientId = System.Guid.NewGuid(), Amount = 1 } },
            Steps = new System.Collections.Generic.List<UpdateRecipeStepRequest> { new UpdateRecipeStepRequest { StepNumber = 1, Instruction = "Do it" } }
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Request.Title);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Model_Is_Valid()
    {
        var command = new UpdateRecipeCommand
        {
            Request = new UpdateRecipeRequest
            {
                Title = "Valid Title",
                Servings = 2,
                PrepTime = 10,
                CookTime = 20,
                Difficulty = RecipeDifficulty.Easy
            },
            Ingredients = new System.Collections.Generic.List<CreateRecipeIngredientRequest> { new CreateRecipeIngredientRequest { IngredientId = System.Guid.NewGuid(), Amount = 1 } },
            Steps = new System.Collections.Generic.List<UpdateRecipeStepRequest> { new UpdateRecipeStepRequest { StepNumber = 1, Instruction = "Do it" } }
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
