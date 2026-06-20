using MediatR;
using OleAI.Backend.Application.Features.Pantry.Constants;
using OleAI.Backend.Application.Features.Pantry.Rules;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.Pantry.Commands.DeleteUserIngredient
{
    public class DeleteUserIngredientCommandHandler : IRequestHandler<DeleteUserIngredientCommand, string>
    {
        private readonly IUserIngredientRepository _userIngredientRepository;
        private readonly PantryBusinessRules _pantryBusinessRules;

        public DeleteUserIngredientCommandHandler(IUserIngredientRepository userIngredientRepository, PantryBusinessRules pantryBusinessRules)
        {
            _userIngredientRepository = userIngredientRepository;
            _pantryBusinessRules = pantryBusinessRules;
        }

        public async Task<string> Handle(DeleteUserIngredientCommand request, CancellationToken cancellationToken)
        {
            var item = await _userIngredientRepository.GetAsync(ui => ui.Id == request.Id && ui.UserId == request.UserId, cancellationToken: cancellationToken);

            if (item == null)
            {
                await _pantryBusinessRules.InventoryItemNotFound(request.Id);
            }

            await _userIngredientRepository.DeleteAsync(item!, permanent: true);

            return PantryMessages.IngredientDeletedMessage;
        }
    }
}
