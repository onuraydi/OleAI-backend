using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using OleAI.Backend.Application.Features.Recipes.Dtos.Requests;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Presentation.ModelBinders;

public abstract class BaseRecipeCommandModelBinder<TCommand> : IModelBinder
{
    protected static readonly JsonSerializerOptions SerializerOptions = new() { PropertyNameCaseInsensitive = true };

    public abstract Task BindModelAsync(ModelBindingContext bindingContext);

    protected async Task<(bool Success, RecipeFormFields? Fields)> ParseCommonFieldsAsync(ModelBindingContext bindingContext)
    {
        var request = bindingContext.HttpContext.Request;

        if (!request.HasFormContentType)
        {
            bindingContext.ModelState.AddModelError(bindingContext.ModelName, "İstek multipart/form-data olmalıdır.");
            bindingContext.Result = ModelBindingResult.Failed();
            return (false, null);
        }

        var form = await request.ReadFormAsync();

        var title = form["Title"].ToString();
        var description = form["Description"].ToString();
        var visibilityText = form["Visibility"].ToString();
        var ingredientsJson = form["IngredientsJson"].ToString();
        var stepsJson = form["StepsJson"].ToString();

        if (!Enum.TryParse<RecipeVisibility>(visibilityText, true, out var visibility))
        {
            bindingContext.ModelState.AddModelError("Visibility", "Visibility alanı geçerli değil. Örnek: Public veya Private.");
        }

        var fields = new RecipeFormFields
        {
            Title = title,
            Description = description,
            Visibility = visibility,
            IngredientsJson = ingredientsJson,
            StepsJson = stepsJson,
            Form = form
        };

        return (true, fields);
    }

    protected List<T> DeserializeJson<T>(string json, string fieldName, ModelBindingContext bindingContext)
    {
        if (string.IsNullOrWhiteSpace(json)) return new List<T>();

        try
        {
            return JsonSerializer.Deserialize<List<T>>(json, SerializerOptions) ?? new List<T>();
        }
        catch (JsonException)
        {
            bindingContext.ModelState.AddModelError(fieldName, $"{fieldName} geçerli JSON formatında olmalıdır.");
            return new List<T>();
        }
    }

    protected static List<int> ParseStepNumbers(StringValues values, ModelBindingContext bindingContext)
    {
        var stepNumbers = new List<int>();
        foreach (var value in values)
        {
            if (string.IsNullOrWhiteSpace(value)) continue;
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

    protected class RecipeFormFields
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public RecipeVisibility Visibility { get; set; }
        public string IngredientsJson { get; set; } = string.Empty;
        public string StepsJson { get; set; } = string.Empty;
        public Microsoft.AspNetCore.Http.IFormCollection Form { get; set; } = default!;
    }
}
