using Microsoft.AspNetCore.Mvc.ModelBinding;
using OleAI.Backend.Application.Features.Recipes.Commands.UpdateRecipe;
using OleAI.Backend.Application.Features.Recipes.Dtos.Requests;

namespace OleAI.Backend.Presentation.ModelBinders;

public class UpdateRecipeCommandModelBinder : BaseRecipeCommandModelBinder<UpdateRecipeCommand>
{
    public override async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

        var (success, fields) = await ParseCommonFieldsAsync(bindingContext);
        if (!success || fields == null) return;

        var newMainImage = fields.Form.Files.GetFile("NewMainImage");

        var ingredients = DeserializeJson<CreateRecipeIngredientRequest>(fields.IngredientsJson, "IngredientsJson", bindingContext);
        var steps = DeserializeJson<UpdateRecipeStepRequest>(fields.StepsJson, "StepsJson", bindingContext);

        var stepImages = fields.Form.Files.Where(f => f.Name == "StepImages").ToList();
        var stepNumbers = ParseStepNumbers(fields.Form["StepImageStepNumbers"], bindingContext);

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
            matchingStep.NewStepImage = stepImages[i];
        }

        if (!bindingContext.ModelState.IsValid)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return;
        }

        var command = new UpdateRecipeCommand
        {
            Request = new UpdateRecipeRequest
            {
                Title = fields.Title,
                Description = fields.Description,
                Visibility = fields.Visibility,
                Difficulty = fields.Difficulty,
                Servings = fields.Servings,
                PrepTime = fields.PrepTime,
                CookTime = fields.CookTime,
                CategoryIds = fields.CategoryIds,
                TagIds = fields.TagIds,
                NewMainImage = newMainImage
            },
            Ingredients = ingredients,
            Steps = steps
        };

        bindingContext.Result = ModelBindingResult.Success(command);
    }
}
