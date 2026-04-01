using OleAI.Backend.Application.Features.IngredientCategories.Constants;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.IngredientCategories.Rules
{
    public class CategoryBusinessRules
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IIngredientRepository _ingredientRepository;

        public CategoryBusinessRules(ICategoryRepository categoryRepository, IIngredientRepository ingredientRepository)
        {
            _categoryRepository = categoryRepository;
            _ingredientRepository = ingredientRepository;
        }

        public void CategoryMustExstWhenRequested(Category? category)
        {
            if (category == null)
                throw new NotFoundException(CategoryMessages.CategoryNotFoundMessage);
        }

        public async Task CategoryNameCannotBeDuplicatedWhenInserted(string name)
        {
            var result = await _categoryRepository.GetAsync(c => c.Name.ToLower() == name.ToLower());
            if (result != null) throw new BusinessException(CategoryMessages.CategoryAlreadyExistsMessage);
        }

        public async Task CategoryCannotBeDeletedIfHasIngredients(Guid categoryId)
        {
            var hasIngredient = await _ingredientRepository.GetAsync(i => i.CategoryId == categoryId);

            if (hasIngredient != null)
                throw new BusinessException(CategoryMessages.CategoryCannotBeDeletedDueToDependenciesMessage);
        }
    }
}
