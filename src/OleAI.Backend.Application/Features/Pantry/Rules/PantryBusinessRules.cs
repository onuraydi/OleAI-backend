using OleAI.Backend.Application.Features.Pantry.Constants;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;

namespace OleAI.Backend.Application.Features.Pantry.Rules
{
    public class PantryBusinessRules
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IUserIngredientRepository _userIngredientRepository;

        public PantryBusinessRules(IIngredientRepository ingredientRepository, IUserIngredientRepository userIngredientRepository)
        {
            _ingredientRepository = ingredientRepository;
            _userIngredientRepository = userIngredientRepository;
        }

        public async Task IngredientMustExistWhenSelectedFromList(Guid ingredientId)
        {
            var ingredient = await _ingredientRepository.GetAsync(i => i.Id == ingredientId);
            if (ingredient == null)
                throw new BusinessException(PantryMessages.IngredientNotFoundMessage);
        }

        public void CannotSendBothIdAndName(Guid? ingredientId, string requestedName)
        {
            if (ingredientId.HasValue && !string.IsNullOrWhiteSpace(requestedName))
                throw new BusinessException(PantryMessages.ProductSelectionConflictMessage);

            if (!ingredientId.HasValue && string.IsNullOrWhiteSpace(requestedName))
                throw new BusinessException(PantryMessages.ProductSelectionRequiredMessage);
        }

        public async Task InventoryItemNotFound(Guid id)
        {
            var userIngredient = await _userIngredientRepository.GetAsync(ui => ui.Id == id);
            if (userIngredient == null)
                throw new NotFoundException(PantryMessages.InventoryItemNotFoundMessage);
        }
    }
}
