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

        public void CategoryMustExistWhenRequested(Category? category)
        {
            if (category == null)
                throw new NotFoundException(CategoryMessages.CategoryNotFoundMessage);
        }

        public async Task CategoryNameCannotBeDuplicatedWhenInserted(string name)
        {
            var result = await _categoryRepository.GetAsync(c => c.Name.ToLower() == name.ToLower());
            if (result != null) throw new BusinessException(CategoryMessages.CategoryAlreadyExistsMessage);
        }

        public async Task CategoryNameCannotBeDuplicatedWhenUpdated(Guid id, string name)
        {
            var result = await _categoryRepository.GetAsync(c => c.Name.ToLower() == name.ToLower() && c.Id != id);
            if (result != null) throw new BusinessException(CategoryMessages.CategoryAlreadyExistsMessage);
        }

        public async Task CategoryCannotBeDeletedIfHasIngredients(Guid categoryId)
        {
            var hasIngredient = await _ingredientRepository.GetAsync(i => i.CategoryId == categoryId);

            if (hasIngredient != null)
                throw new BusinessException(CategoryMessages.CategoryCannotBeDeletedDueToDependenciesMessage);
        }

        public async Task CheckIfCategoriesExistAsync(List<Guid> categoryIds, CancellationToken cancellationToken = default)
        {
            var count = await _categoryRepository.GetCountAsync(
                predicate: c => categoryIds.Contains(c.Id),
                cancellationToken: cancellationToken);

            if (count != categoryIds.Count)
                throw new NotFoundException("Belirtilen kategorilerden bazıları bulunamadı.");
        }
    }
}
