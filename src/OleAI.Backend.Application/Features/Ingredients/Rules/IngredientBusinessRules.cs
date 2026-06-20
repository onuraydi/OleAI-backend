using OleAI.Backend.Application.Features.Ingredients.Constants;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Ingredients.Rules
{
    public class IngredientBusinessRules
    {
        private readonly IIngredientRepository _ingredientRepository;

        public IngredientBusinessRules(IIngredientRepository ingredientRepository)
        {
            _ingredientRepository = ingredientRepository;
        }

        public void IngredientMustExistWhenRequested(Ingredient? ingredient)
        {
            if (ingredient == null) throw new NotFoundException(IngredientMessages.IngredientNotFoundMessage);
        }

        public async Task IngredientNameCannotBeDuplicatedWhenInserted(string name)
        {
            var result = await _ingredientRepository.GetAsync(i => i.Name.ToLower() == name.ToLower() && i.DeletedDate == null);
            if (result != null) throw new BusinessException(IngredientMessages.ActiveIngredientAlreadyExistsMessage);
        }

        public async Task IngredientNameCannotBeDuplicatedWhenUpdated(Guid id, string name)
        {
            var result = await _ingredientRepository.GetAsync(i => i.Id != id && i.Name.ToLower() == name.ToLower() && i.DeletedDate == null);
            if (result != null) throw new BusinessException(IngredientMessages.ActiveIngredientAlreadyExistsMessage);
        }

        public async Task CheckIfIngredientsExistAsync(List<Guid> ingredientIds, CancellationToken cancellationToken = default)
        {
            var count = await _ingredientRepository.GetCountAsync(
                predicate: i => ingredientIds.Contains(i.Id),
                cancellationToken: cancellationToken);

            if (count != ingredientIds.Count)
                throw new NotFoundException("Belirtilen malzemelerden bazıları bulunamadı.");
        }
    }
}
