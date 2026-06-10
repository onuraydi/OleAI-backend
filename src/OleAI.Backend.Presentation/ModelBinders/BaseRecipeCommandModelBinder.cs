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
        var difficultyText = form["Difficulty"].ToString();
        var ingredientsJson = form["IngredientsJson"].ToString();
        var stepsJson = form["StepsJson"].ToString();

        if (!Enum.TryParse<RecipeVisibility>(visibilityText, true, out var visibility))
        {
            bindingContext.ModelState.AddModelError("Visibility", "Visibility alanı geçerli değil. Örnek: Public veya Private.");
        }

        if (!Enum.TryParse<RecipeDifficulty>(difficultyText, true, out var difficulty))
        {
            bindingContext.ModelState.AddModelError("Difficulty", "Difficulty alanı geçerli değil.");
        }

        int.TryParse(form["Servings"].ToString(), out var servings);
        int.TryParse(form["PrepTime"].ToString(), out var prepTime);
        int.TryParse(form["CookTime"].ToString(), out var cookTime);

        var categoryIds = ParseIntList(form["CategoryIds"], bindingContext, "CategoryIds");
        var tagIds = ParseIntList(form["TagIds"], bindingContext, "TagIds");

        var fields = new RecipeFormFields
        {
            Title = title,
            Description = description,
            Visibility = visibility,
            Difficulty = difficulty,
            Servings = servings,
            PrepTime = prepTime,
            CookTime = cookTime,
            CategoryIds = categoryIds,
            TagIds = tagIds,
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

    protected static List<int> ParseIntList(StringValues values, ModelBindingContext bindingContext, string fieldName)
    {
        var result = new List<int>();
        foreach (var value in values)
        {
            if (string.IsNullOrWhiteSpace(value)) continue;
            var tokens = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var token in tokens)
            {
                if (int.TryParse(token, out var parsedValue))
                {
                    result.Add(parsedValue);
                }
                else
                {
                    bindingContext.ModelState.AddModelError(fieldName, $"{fieldName} sayısal değerler içermelidir.");
                }
            }
        }
        return result;
    }

    protected class RecipeFormFields
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public RecipeVisibility Visibility { get; set; }
        public RecipeDifficulty Difficulty { get; set; }
        public int Servings { get; set; }
        public int PrepTime { get; set; }
        public int CookTime { get; set; }
        public List<int> CategoryIds { get; set; } = new();
        public List<int> TagIds { get; set; } = new();
        public string IngredientsJson { get; set; } = string.Empty;
        public string StepsJson { get; set; } = string.Empty;
        public Microsoft.AspNetCore.Http.IFormCollection Form { get; set; } = default!;
    }
}
