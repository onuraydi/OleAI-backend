using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.DietaryPreferences.Constants;
using OleAI.Backend.Application.Features.DietaryPreferences.Rules;
using OleAI.Backend.Application.Features.IngredientCategories.Rules;
using OleAI.Backend.Application.Features.Ingredients.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.DietaryPreferences.Commands.UpdateDietaryPreference;

public class UpdateDietaryPreferenceCommandHandler : IRequestHandler<UpdateDietaryPreferenceCommand, string>
{
    private readonly IDietaryPreferenceRepository _dietaryPreferenceRepository;
    private readonly DietaryPreferenceBusinessRules _businessRules;
    private readonly IngredientBusinessRules _ingredientRules;
    private readonly CategoryBusinessRules _categoryRules;

    public UpdateDietaryPreferenceCommandHandler(
        IDietaryPreferenceRepository dietaryPreferenceRepository,
        DietaryPreferenceBusinessRules businessRules,
        IngredientBusinessRules ingredientRules,
        CategoryBusinessRules categoryRules)
    {
        _dietaryPreferenceRepository = dietaryPreferenceRepository;
        _businessRules = businessRules;
        _ingredientRules = ingredientRules;
        _categoryRules = categoryRules;
    }

    public async Task<string> Handle(UpdateDietaryPreferenceCommand request, CancellationToken cancellationToken)
    {
        var dietaryPreference = await _dietaryPreferenceRepository.GetAsync(
            predicate: dp => dp.Id == request.Id,
            include: q => q
                .Include(dp => dp.ExcludedIngredients)
                .Include(dp => dp.ExcludedCategories),
            cancellationToken: cancellationToken);

        _businessRules.CheckIfDietaryPreferenceExists(dietaryPreference);
        await _businessRules.CheckIfDietaryPreferenceNameExistsWhenUpdatingAsync(request.Name, request.Id, cancellationToken);

        if (request.ExcludedIngredientIds.Count > 0)
            await _ingredientRules.CheckIfIngredientsExistAsync(request.ExcludedIngredientIds, cancellationToken);

        if (request.ExcludedCategoryIds.Count > 0)
            await _categoryRules.CheckIfCategoriesExistAsync(request.ExcludedCategoryIds, cancellationToken);

        dietaryPreference!.Name = request.Name.Trim();
        dietaryPreference.Desciption = request.Description?.Trim();
        dietaryPreference.IsActive = request.IsActive;

        // Upsert for Excluded Ingredients
        dietaryPreference.ExcludedIngredients.Clear();
        foreach (var ingredientId in request.ExcludedIngredientIds)
        {
            dietaryPreference.ExcludedIngredients.Add(new DietaryPreferenceExcludedIngredient
            {
                DietaryPreferenceId = request.Id,
                IngredientId = ingredientId
            });
        }

        // Upsert for Excluded Categories
        dietaryPreference.ExcludedCategories.Clear();
        foreach (var categoryId in request.ExcludedCategoryIds)
        {
            dietaryPreference.ExcludedCategories.Add(new DietaryPreferenceExcludedCategory
            {
                DietaryPreferenceId = request.Id,
                CategoryId = categoryId
            });
        }

        await _dietaryPreferenceRepository.UpdateAsync(dietaryPreference, cancellationToken);

        return DietaryPreferenceMessages.DietaryPreferenceUpdatedSuccessfully;
    }
}
