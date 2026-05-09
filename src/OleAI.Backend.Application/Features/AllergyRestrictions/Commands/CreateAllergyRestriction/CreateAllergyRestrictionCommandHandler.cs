using MediatR;
using OleAI.Backend.Application.Features.AllergyRestrictions.Constants;
using OleAI.Backend.Application.Features.AllergyRestrictions.Rules;
using OleAI.Backend.Application.Features.IngredientCategories.Rules;
using OleAI.Backend.Application.Features.Ingredients.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.AllergyRestrictions.Commands.CreateAllergyRestriction;

public class CreateAllergyRestrictionCommandHandler : IRequestHandler<CreateAllergyRestrictionCommand, string>
{
    private readonly IAllergyRestrictionRepository _allergyRestrictionRepository;
    private readonly AllergyRestrictionBusinessRules _businessRules;
    private readonly IngredientBusinessRules _ingredientRules;
    private readonly CategoryBusinessRules _categoryRules;

    public CreateAllergyRestrictionCommandHandler(
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

    public async Task<string> Handle(CreateAllergyRestrictionCommand request, CancellationToken cancellationToken)
    {
        await _businessRules.CheckIfAllergyRestrictionNameExistsAsync(request.Name, cancellationToken);

        if (request.ExcludedIngredientIds.Count > 0)
            await _ingredientRules.CheckIfIngredientsExistAsync(request.ExcludedIngredientIds, cancellationToken);

        if (request.ExcludedCategoryIds.Count > 0)
            await _categoryRules.CheckIfCategoriesExistAsync(request.ExcludedCategoryIds, cancellationToken);

        var restriction = new AllergyRestriction
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            IsActive = true
        };

        foreach (var ingredientId in request.ExcludedIngredientIds)
        {
            restriction.ExcludedIngredients.Add(new AllergyExcludedIngredient
            {
                IngredientId = ingredientId
            });
        }

        foreach (var categoryId in request.ExcludedCategoryIds)
        {
            restriction.ExcludedCategories.Add(new AllergyExcludedCategory
            {
                CategoryId = categoryId
            });
        }

        await _allergyRestrictionRepository.AddAsync(restriction, cancellationToken);

        return AllergyRestrictionMessages.AllergyRestrictionCreatedSuccessfully;
    }
}
