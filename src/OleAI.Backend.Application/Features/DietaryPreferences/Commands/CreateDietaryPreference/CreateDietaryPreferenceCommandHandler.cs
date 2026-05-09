using MediatR;
using OleAI.Backend.Application.Features.DietaryPreferences.Constants;
using OleAI.Backend.Application.Features.DietaryPreferences.Rules;
using OleAI.Backend.Application.Features.IngredientCategories.Rules;
using OleAI.Backend.Application.Features.Ingredients.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.DietaryPreferences.Commands.CreateDietaryPreference;

public class CreateDietaryPreferenceCommandHandler : IRequestHandler<CreateDietaryPreferenceCommand, string>
{
    private readonly IDietaryPreferenceRepository _dietaryPreferenceRepository;
    private readonly DietaryPreferenceBusinessRules _businessRules;
    private readonly IngredientBusinessRules _ingredientRules;
    private readonly CategoryBusinessRules _categoryRules;

    public CreateDietaryPreferenceCommandHandler(
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

    public async Task<string> Handle(CreateDietaryPreferenceCommand request, CancellationToken cancellationToken)
    {
        await _businessRules.CheckIfDietaryPreferenceNameExistsAsync(request.Name, cancellationToken);

        // İlgili malzeme ve kategori ID'leri db'de var mı kontrolü (Clean Architecture)
        if (request.ExcludedIngredientIds.Count > 0)
        {
            await _ingredientRules.CheckIfIngredientsExistAsync(request.ExcludedIngredientIds, cancellationToken);
        }

        if (request.ExcludedCategoryIds.Count > 0)
        {
            await _categoryRules.CheckIfCategoriesExistAsync(request.ExcludedCategoryIds, cancellationToken);
        }

        var dietaryPreference = new DietaryPreference
        {
            Name = request.Name.Trim(),
            Desciption = request.Description?.Trim(),
            IsActive = true
        };

        foreach (var ingredientId in request.ExcludedIngredientIds)
        {
            dietaryPreference.ExcludedIngredients.Add(new DietaryPreferenceExcludedIngredient
            {
                IngredientId = ingredientId
            });
        }

        foreach (var categoryId in request.ExcludedCategoryIds)
        {
            dietaryPreference.ExcludedCategories.Add(new DietaryPreferenceExcludedCategory
            {
                CategoryId = categoryId
            });
        }

        await _dietaryPreferenceRepository.AddAsync(dietaryPreference, cancellationToken);

        return DietaryPreferenceMessages.DietaryPreferenceCreatedSuccessfully;
    }
}
