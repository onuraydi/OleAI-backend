using MediatR;
using OleAI.Backend.Application.Features.Pantry.Constants;
using OleAI.Backend.Application.Features.Pantry.Rules;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.Pantry.Commands.CancelRequest
{
    public class CancelIngredientRequestCommandHandler : IRequestHandler<CancelIngredientRequestCommand, string>
    {
        private readonly IIngredientRequestRepository _ingredientRequestRepository;
        private readonly IUserIngredientRepository _userIngredientRepository;
        private readonly PantryBusinessRules _pantryBusinessRules;

        public CancelIngredientRequestCommandHandler(IIngredientRequestRepository ingredientRequestRepository, IUserIngredientRepository userIngredientRepository, PantryBusinessRules pantryBusinessRules)
        {
            _ingredientRequestRepository = ingredientRequestRepository;
            _userIngredientRepository = userIngredientRepository;
            _pantryBusinessRules = pantryBusinessRules;
        }

        public async Task<string> Handle(CancelIngredientRequestCommand request, CancellationToken cancellationToken)
        {
            var ingredientRequest = await _ingredientRequestRepository.GetAsync(ir => ir.Id == request.RequestId && ir.UserId == request.UserId, cancellationToken: cancellationToken);

            _pantryBusinessRules.IngredientRequestMustExistWhenSelected(ingredientRequest!);
            _pantryBusinessRules.CannotBeCancelledRequest(ingredientRequest!.Status);

            var pendingUserInventoryItem = await _userIngredientRepository.GetAsync(ui => ui.IngredientRequestId == request.RequestId);
            if (pendingUserInventoryItem != null)
            {
                await _userIngredientRepository.DeleteAsync(pendingUserInventoryItem, permanent: true);
            }

            ingredientRequest.Reject(PantryMessages.RequestCancelledByUserMessage);
            await _ingredientRequestRepository.UpdateAsync(ingredientRequest);

            return PantryMessages.IngredientRequestCancelledMessage;
        }
    }
}
