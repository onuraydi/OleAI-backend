using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.AllergyRestrictions.Constants;
using OleAI.Backend.Application.Features.AllergyRestrictions.Rules;
using OleAI.Backend.Application.Features.IngredientCategories.Rules;
using OleAI.Backend.Application.Features.Ingredients.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.AllergyRestrictions.Commands.UpdateAllergyRestriction;

public class UpdateAllergyRestrictionCommandHandler : IRequestHandler<UpdateAllergyRestrictionCommand, string>
{
    private readonly IAllergyRestrictionRepository _allergyRestrictionRepository;
    private readonly AllergyRestrictionBusinessRules _businessRules;
    private readonly IngredientBusinessRules _ingredientRules;
    private readonly CategoryBusinessRules _categoryRules;

    public UpdateAllergyRestrictionCommandHandler(
        IAllergyRestrictionRepository allergyRestrictionRepository,
        AllergyRestrictionBusinessRules businessRules,
        IngredientBusinessRules ingredientRules,
        CategoryBusinessRules categoryRules)
    {
        _allergyRestrictionRepository = allergyRestrictionRepository;
        _businessRules = businessRules;
        _ingredientRules = ingredientRules;
        _categoryRules = categoryRules;
    }

    public async Task<string> Handle(UpdateAllergyRestrictionCommand request, CancellationToken cancellationToken)
    {
        var restriction = await _allergyRestrictionRepository.GetAsync(
            predicate: ar => ar.Id == request.Id,
            include: q => q
                .Include(ar => ar.ExcludedIngredients)
                .Include(ar => ar.ExcludedCategories),
            cancellationToken: cancellationToken);

        _businessRules.CheckIfAllergyRestrictionExists(restriction);
        await _businessRules.CheckIfAllergyRestrictionNameExistsWhenUpdatingAsync(request.Name, request.Id, cancellationToken);

        if (request.ExcludedIngredientIds.Count > 0)
            await _ingredientRules.CheckIfIngredientsExistAsync(request.ExcludedIngredientIds, cancellationToken);

        if (request.ExcludedCategoryIds.Count > 0)
            await _categoryRules.CheckIfCategoriesExistAsync(request.ExcludedCategoryIds, cancellationToken);

        restriction!.Name = request.Name.Trim();
        restriction.Description = request.Description?.Trim();
        restriction.IsActive = request.IsActive;

        // Upsert for Excluded Ingredients
        restriction.ExcludedIngredients.Clear();
        foreach (var ingredientId in request.ExcludedIngredientIds)
        {
            restriction.ExcludedIngredients.Add(new AllergyExcludedIngredient
            {
                AllergyRestrictionId = request.Id,
                IngredientId = ingredientId
            });
        }

        // Upsert for Excluded Categories
        restriction.ExcludedCategories.Clear();
        foreach (var categoryId in request.ExcludedCategoryIds)
        {
            restriction.ExcludedCategories.Add(new AllergyExcludedCategory
            {
                AllergyRestrictionId = request.Id,
                CategoryId = categoryId
            });
        }

        await _allergyRestrictionRepository.UpdateAsync(restriction, cancellationToken);

        return AllergyRestrictionMessages.AllergyRestrictionUpdatedSuccessfully;
    }
}
