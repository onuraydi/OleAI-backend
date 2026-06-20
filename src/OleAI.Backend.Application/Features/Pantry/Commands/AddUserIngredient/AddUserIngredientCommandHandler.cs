using MediatR;
using OleAI.Backend.Application.Features.Pantry.Constants;
using OleAI.Backend.Application.Features.Pantry.Dtos.Responses;
using OleAI.Backend.Application.Features.Pantry.Events;
using OleAI.Backend.Application.Features.Pantry.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Pantry.Commands.AddUserIngredient
{
    public class AddUserIngredientCommandHandler : IRequestHandler<AddUserIngredientCommand, AddedUserIngredientResponse>
    {
        private readonly IUserIngredientRepository _userIngredientRepository;
        private readonly IIngredientRequestRepository _ingredientRequestRepository;
        private readonly PantryBusinessRules _businessRules;
        private readonly IMediator _mediator;

        public AddUserIngredientCommandHandler(IUserIngredientRepository userIngredientRepository, IIngredientRequestRepository ingredientRequestRepository, PantryBusinessRules businessRules, IMediator mediator)
        {
            _userIngredientRepository = userIngredientRepository;
            _ingredientRequestRepository = ingredientRequestRepository;
            _businessRules = businessRules;
            _mediator = mediator;
        }

        public async Task<AddedUserIngredientResponse> Handle(AddUserIngredientCommand command, CancellationToken cancellationToken)
        {
            _businessRules.CannotSendBothIdAndName(command.Request.IngredientId, command.Request.RequestedName!);

            UserIngredient userIngredient;
            bool isPending = false;
            string message;

            // Sistemde olan (master) bir ürün eklemek
            if (command.Request.IngredientId.HasValue)
            {
                var ingredient = await _businessRules.IngredientMustExistWhenSelectedFromList(command.Request.IngredientId!.Value);

                var existingItem = await _userIngredientRepository.GetAsync(
                    predicate: ui => ui.UserId == command.Request.UserId && ui.IngredientId == command.Request.IngredientId.Value && ui.IngredientAliasId == command.Request.AliasId,
                    cancellationToken: cancellationToken
                    );

                DateTime? finalExpirationDate = command.Request.ExpirationDate;
                if (!finalExpirationDate.HasValue && ingredient.EstimatedShelfLifeInDays.HasValue)
                {
                    finalExpirationDate = DateTime.UtcNow.AddDays(ingredient.EstimatedShelfLifeInDays.Value);
                }

                if (existingItem != null)
                {
                    existingItem.AddQuantity(command.Request.Quantity);
                    if (finalExpirationDate.HasValue)
                    {
                        existingItem.UpdateExpirationDate(finalExpirationDate);
                    }
                    await _userIngredientRepository.UpdateAsync(existingItem, cancellationToken);
                    userIngredient = existingItem;
                    message = PantryMessages.IngredientQuantityUpdatedMessage;
                }
                else
                {
                    userIngredient = UserIngredient.CreateStandard(Guid.NewGuid(), command.Request.UserId, command.Request.IngredientId.Value, command.Request.Quantity, command.Request.Unit, finalExpirationDate, command.Request.AliasId);
                    await _userIngredientRepository.AddAsync(userIngredient, cancellationToken);
                    message = PantryMessages.IngredientAddedPantryMessage;
                }
            }

            // Sistem olmayan bir ürünü kullanıcı serbest metinle yazdığı zaman // Direkt eklensin ancak onay bekliyor gözükmesin
            else
            {
                var ingredientRequest = new IngredientRequest(Guid.NewGuid(), command.Request.UserId, command.Request.RequestedName!, command.Request.SuggestedCategoryId, command.Request.EstimatedShelfLifeInDays);
                await _ingredientRequestRepository.AddAsync(ingredientRequest, cancellationToken);

                userIngredient = UserIngredient.CreatePending(Guid.NewGuid(), command.Request.UserId, ingredientRequest.Id, command.Request.Quantity, command.Request.Unit, command.Request.ExpirationDate);
                await _userIngredientRepository.AddAsync(userIngredient, cancellationToken);

                isPending = true;
                message = PantryMessages.IngredientSentForAiApprovalMessage;

                await _mediator.Publish(new IngredientRequestedEvent
                {
                    RequestId = ingredientRequest.Id,
                    RequestedName = ingredientRequest.RequestedName!,
                }, cancellationToken);
            }

            return new AddedUserIngredientResponse
            {
                Id = userIngredient.Id,
                IsPendingApproval = isPending,
                Message = message,
            };
        }
    }
}
