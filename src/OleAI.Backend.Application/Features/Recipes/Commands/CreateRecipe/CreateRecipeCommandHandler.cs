using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.ImageStorage;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Recipes.Commands.CreateRecipe
{
    public class CreateRecipeCommandHandler : IRequestHandler<CreateRecipeCommand, string>
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IImageService _imageService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RecipeBusinessRules _recipeBusinessRules;

        public CreateRecipeCommandHandler(IRecipeRepository recipeRepository, IImageService imageService, IHttpContextAccessor httpContextAccessor, RecipeBusinessRules recipeBusinessRules)
        {
            _recipeRepository = recipeRepository;
            _imageService = imageService;
            _httpContextAccessor = httpContextAccessor;
            _recipeBusinessRules = recipeBusinessRules;
        }

        public async Task<string> Handle(CreateRecipeCommand command, CancellationToken cancellationToken)
        {
            var requestedIngredientIds = command.Ingredients.Select(i => i.IngredientId).Distinct().ToList();
            await _recipeBusinessRules.CheckIfIngredientsExistAsync(requestedIngredientIds, cancellationToken);

            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException(RecipeMessages.UnauthorizedAccess);

            var userId = Guid.Parse(userIdClaim);

            var recipe = Recipe.Create(userId, command.CreateRecipeRequest.Title, command.CreateRecipeRequest.Description, command.CreateRecipeRequest.Visibility);

            var mainImageUpload = await _imageService.UploadAsync(command.CreateRecipeRequest.MainImage, "recipes/main", cancellationToken);
            recipe.AddImage(mainImageUpload.Url, mainImageUpload.PublicId, isMain: true);

            foreach (var ingredientDto in command.Ingredients)
            {
                recipe.AddIngredient(ingredientDto.IngredientId, ingredientDto.Amount, ingredientDto.UnitId);
            }

            var orderedSteps = command.Steps.OrderBy(s => s.StepNumber).ToList();
            for (int i = 0; i < orderedSteps.Count; i++)
            {
                var stepDto = orderedSteps[i];
                int currentStepNumber = i + 1;

                recipe.AddStep(currentStepNumber, stepDto.Instruction);

                if (stepDto.StepImage != null)
                {
                    var addedStep = recipe.RecipeSteps.FirstOrDefault(s => s.StepNumber == currentStepNumber);
                    if (addedStep != null)
                    {
                        var stepImageUpload = await _imageService.UploadAsync(stepDto.StepImage, "recipes/steps", cancellationToken);
                        recipe.AddImage(stepImageUpload.Url, stepImageUpload.PublicId, isMain: false, stepId: addedStep.Id);
                    }
                }
            }

            await _recipeRepository.AddAsync(recipe);

            return RecipeMessages.RecipeAddedSuccessfully;
        }
    }
}
