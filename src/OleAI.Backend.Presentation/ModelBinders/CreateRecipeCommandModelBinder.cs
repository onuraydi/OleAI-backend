using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using OleAI.Backend.Application.Features.Recipes.Commands.CreateRecipe;
using OleAI.Backend.Application.Features.Recipes.Dtos.Requests;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Presentation.ModelBinders;

public class CreateRecipeCommandModelBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        var request = bindingContext.HttpContext.Request;

        if (!request.HasFormContentType)
        {
            bindingContext.ModelState.AddModelError(bindingContext.ModelName, "İstek multipart/form-data olmalıdır.");
            bindingContext.Result = ModelBindingResult.Failed();
            return;
        }

        var form = await request.ReadFormAsync();

        var title = form["Title"].ToString();
        var description = form["Description"].ToString();
        var visibilityText = form["Visibility"].ToString();
        var ingredientsJson = form["IngredientsJson"].ToString();
        var stepsJson = form["StepsJson"].ToString();

        var mainImage = form.Files.GetFile("MainImage");

        var serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        List<CreateRecipeIngredientRequest> ingredients = new();
        List<CreateRecipeStepRequest> steps = new();

        if (!string.IsNullOrWhiteSpace(ingredientsJson))
        {
            try
            {
                ingredients = JsonSerializer.Deserialize<List<CreateRecipeIngredientRequest>>(ingredientsJson, serializerOptions) ?? new();
            }
            catch (JsonException)
            {
                bindingContext.ModelState.AddModelError("IngredientsJson", "IngredientsJson geçerli JSON formatında olmalıdır.");
            }
        }

        if (!string.IsNullOrWhiteSpace(stepsJson))
        {
            try
            {
                steps = JsonSerializer.Deserialize<List<CreateRecipeStepRequest>>(stepsJson, serializerOptions) ?? new();
            }
            catch (JsonException)
            {
                bindingContext.ModelState.AddModelError("StepsJson", "StepsJson geçerli JSON formatında olmalıdır.");
            }
        }

        if (!Enum.TryParse<RecipeVisibility>(visibilityText, true, out var visibility))
        {
            bindingContext.ModelState.AddModelError("Visibility", "Visibility alanı geçerli değil. Örnek: Public veya Private.");
        }

        if (mainImage == null)
        {
            bindingContext.ModelState.AddModelError("MainImage", "Tarifin ana kapak fotoğrafı zorunludur.");
        }

        var stepImages = form.Files.Where(f => f.Name == "StepImages").ToList();
        var stepNumberValues = form["StepImageStepNumbers"];
        var stepNumbers = ParseStepNumbers(stepNumberValues, bindingContext);

        if (stepImages.Count != stepNumbers.Count)
        {
            bindingContext.ModelState.AddModelError("StepImages", "StepImages ve StepImageStepNumbers sayıları eşleşmelidir.");
        }

        for (int i = 0; i < Math.Min(stepImages.Count, stepNumbers.Count); i++)
        {
            var stepNumber = stepNumbers[i];
            var matchingStep = steps.FirstOrDefault(s => s.StepNumber == stepNumber);
            if (matchingStep == null)
            {
                bindingContext.ModelState.AddModelError("StepImageStepNumbers", $"{stepNumber} numaralı adım StepsJson içinde bulunamadı.");
                continue;
            }

            matchingStep.StepImage = stepImages[i];
        }

        if (!bindingContext.ModelState.IsValid)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return;
        }

        var command = new CreateRecipeCommand
        {
            CreateRecipeRequest = new CreateRecipeRequest
            {
                Title = title,
                Description = description,
                Visibility = visibility,
                MainImage = mainImage!
            },
            Ingredients = ingredients,
            Steps = steps
        };

        bindingContext.Result = ModelBindingResult.Success(command);
    }

    private static List<int> ParseStepNumbers(StringValues values, ModelBindingContext bindingContext)
    {
        var stepNumbers = new List<int>();
        foreach (var value in values)
        {
            var tokens = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var token in tokens)
            {
                if (int.TryParse(token, out var stepNumber))
                {
                    stepNumbers.Add(stepNumber);
                }
                else
                {
                    bindingContext.ModelState.AddModelError("StepImageStepNumbers", "StepImageStepNumbers sayısal değerler içermelidir.");
                }
            }
        }

        return stepNumbers;
    }
}
