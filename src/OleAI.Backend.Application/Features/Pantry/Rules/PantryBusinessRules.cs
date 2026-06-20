using OleAI.Backend.Application.Features.Pantry.Constants;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Pantry.Rules
{
    public class PantryBusinessRules
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IUserIngredientRepository _userIngredientRepository;
        private readonly IIngredientRequestRepository _ingredientRequestRepository;

        public PantryBusinessRules(IIngredientRepository ingredientRepository, IUserIngredientRepository userIngredientRepository, IIngredientRequestRepository ingredientRequestRepository)
        {
            _ingredientRepository = ingredientRepository;
            _userIngredientRepository = userIngredientRepository;
            _ingredientRequestRepository = ingredientRequestRepository;
        }

        public async Task<Ingredient> IngredientMustExistWhenSelectedFromList(Guid ingredientId)
        {
            var ingredient = await _ingredientRepository.GetAsync(i => i.Id == ingredientId);
            if (ingredient == null)
                throw new BusinessException(PantryMessages.IngredientNotFoundMessage);
            return ingredient;
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

        public void IngredientRequestMustExistWhenSelected(IngredientRequest request)
        {
            if (request == null)
                throw new NotFoundException(PantryMessages.IngredientRequestNotFoundMessage);
        }

        public void CannotBeCancelledRequest(IngredientRequestStatus status)
        {
            if (status != IngredientRequestStatus.Pending)
                throw new BusinessException(PantryMessages.RequestAlreadyProcessedCannotBeCancelledMessage);
        }

        public void ExpirationDateCannotBeInThePast(DateTime? expirationDate)
        {
            if (expirationDate.HasValue && expirationDate.Value.Date < DateTime.UtcNow.Date)
                throw new BusinessException(PantryMessages.ExpirationDateCannotBeInThePastMessage);
        }
    }
}
