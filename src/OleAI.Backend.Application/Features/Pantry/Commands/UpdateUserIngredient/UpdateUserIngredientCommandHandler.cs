using MediatR;
using OleAI.Backend.Application.Features.Pantry.Constants;
using OleAI.Backend.Application.Features.Pantry.Rules;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.Pantry.Commands.UpdateUserIngredient
{
    public class UpdateUserIngredientCommandHandler : IRequestHandler<UpdateUserIngredientCommand, string>
    {
        private readonly IUserIngredientRepository _userIngredientRepository;
        private readonly PantryBusinessRules _pantryBusinessRules;

        public UpdateUserIngredientCommandHandler(IUserIngredientRepository userIngredientRepository, PantryBusinessRules pantryBusinessRules)
        {
            _userIngredientRepository = userIngredientRepository;
            _pantryBusinessRules = pantryBusinessRules;
        }

        public async Task<string> Handle(UpdateUserIngredientCommand command, CancellationToken cancellationToken)
        {
            var item = await _userIngredientRepository.GetAsync(i => i.Id == command.Request.Id && i.UserId == command.Request.UserId, cancellationToken: cancellationToken);

            if (item == null)
            {
                await _pantryBusinessRules.InventoryItemNotFound(command.Request.Id); // Trigger the expected exception
            }

            item!.UpdateQuantity(command.Request.NewQuantity, command.Request.NewUnit);
            await _userIngredientRepository.UpdateAsync(item);

            return PantryMessages.IngredientQuantityUpdatedMessage;
        }
    }
}
