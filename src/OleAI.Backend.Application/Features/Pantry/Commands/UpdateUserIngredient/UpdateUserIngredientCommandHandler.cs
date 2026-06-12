using MediatR;
using OleAI.Backend.Application.Features.Pantry.Constants;
using OleAI.Backend.Application.Features.Pantry.Events;
using OleAI.Backend.Application.Features.Pantry.Rules;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.Pantry.Commands.UpdateUserIngredient
{
    public class UpdateUserIngredientCommandHandler : IRequestHandler<UpdateUserIngredientCommand, string>
    {
        private const double ZeroAmountTolerance = 0.0001d;

        private readonly IUserIngredientRepository _userIngredientRepository;
        private readonly PantryBusinessRules _pantryBusinessRules;
        private readonly IMediator _mediator;

        public UpdateUserIngredientCommandHandler(
            IUserIngredientRepository userIngredientRepository,
            PantryBusinessRules pantryBusinessRules,
            IMediator mediator)
        {
            _userIngredientRepository = userIngredientRepository;
            _pantryBusinessRules = pantryBusinessRules;
            _mediator = mediator;
        }

        public async Task<string> Handle(UpdateUserIngredientCommand command, CancellationToken cancellationToken)
        {
            var item = await _userIngredientRepository.GetAsync(
                i => i.Id == command.Request.Id && i.UserId == command.Request.UserId,
                cancellationToken: cancellationToken);

            if (item == null)
                await _pantryBusinessRules.InventoryItemNotFound(command.Request.Id);

            item!.UpdateQuantity(command.Request.NewQuantity, command.Request.NewUnit ?? item.Unit);

            if (command.Request.ExpirationDate.HasValue || command.Request.ExpirationDate == null)
            {
                _pantryBusinessRules.ExpirationDateCannotBeInThePast(command.Request.ExpirationDate);
                // UI'dan gelen tarihe guncelle (veya temizle)
                item.UpdateExpirationDate(command.Request.ExpirationDate);
            }

            // Miktar 0'a düşünce (kullanıcı manuel sıfırladı) alışveriş listesini tetikle
            if (Math.Abs(item.Amount) <= ZeroAmountTolerance && item.IngredientId.HasValue)
            {
                await _userIngredientRepository.DeleteAsync(item, permanent: true, cancellationToken: cancellationToken);

                await _mediator.Publish(new IngredientDepletedEvent
                {
                    UserId = item.UserId,
                    MasterIngredientId = item.IngredientId.Value,
                    IngredientName = item.Ingredient?.Name ?? string.Empty
                }, cancellationToken);

                return PantryMessages.IngredientDepletedAndRemovedMessage;
            }

            await _userIngredientRepository.UpdateAsync(item, cancellationToken);

            return PantryMessages.IngredientQuantityUpdatedMessage;
        }
    }
}
