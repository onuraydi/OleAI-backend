using FluentAssertions;
using OleAI.Backend.Application.Features.Recipes.Commands.CreateRecipe;
using OleAI.Backend.Application.Features.Recipes.Dtos.Requests;
using OleAI.Backend.Application.Tests.TestInfrastructure;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Validations;

public class CreateRecipeCommandValidatorTests
{
    private readonly CreateRecipeCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldPass()
    {
        var command = new CreateRecipeCommand
        {
            CreateRecipeRequest = new CreateRecipeRequest
            {
                Title = "Mercimek Çorbası",
                Description = "Klasik tarif",
                Visibility = RecipeVisibility.Public,
                MainImage = RecipeTestHelpers.CreateFormFile()
            },
            Ingredients =
            [
                new CreateRecipeIngredientRequest { IngredientId = Guid.NewGuid(), Amount = 1, UnitId = 1 }
            ],
            Steps =
            [
                new CreateRecipeStepRequest { StepNumber = 1, Instruction = "Karıştır" }
            ]
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenTitleIsEmpty_ShouldReturnError()
    {
        var command = BuildBaseCommand();
        command.CreateRecipeRequest.Title = string.Empty;

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Select(e => e.ErrorMessage).Should().Contain("Tarif adı boş olamaz.");
    }

    [Fact]
    public void Validate_WhenMainImageIsMissing_ShouldReturnError()
    {
        var command = BuildBaseCommand();
        command.CreateRecipeRequest.MainImage = null!;

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Select(e => e.ErrorMessage).Should().Contain("Tarifin ana kapak fotoğrafı zorunludur.");
    }

    [Fact]
    public void Validate_WhenIngredientAmountIsZero_ShouldReturnError()
    {
        var command = BuildBaseCommand();
        command.Ingredients[0].Amount = 0;

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Select(e => e.ErrorMessage).Should().Contain("Miktar 0'dan büyük olmalıdır.");
    }

    [Fact]
    public void Validate_WhenStepInstructionIsEmpty_ShouldReturnError()
    {
        var command = BuildBaseCommand();
        command.Steps[0].Instruction = string.Empty;

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Select(e => e.ErrorMessage).Should().Contain("Adım açıklaması boş olamaz.");
    }

    private static CreateRecipeCommand BuildBaseCommand()
    {
        return new CreateRecipeCommand
        {
            CreateRecipeRequest = new CreateRecipeRequest
            {
                Title = "Mercimek Çorbası",
                Description = "Klasik tarif",
                Visibility = RecipeVisibility.Public,
                MainImage = RecipeTestHelpers.CreateFormFile()
            },
            Ingredients =
            [
                new CreateRecipeIngredientRequest { IngredientId = Guid.NewGuid(), Amount = 1, UnitId = 1 }
            ],
            Steps =
            [
                new CreateRecipeStepRequest { StepNumber = 1, Instruction = "Karıştır" }
            ]
        };
    }
}
