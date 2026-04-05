using MediatR;
using OleAI.Backend.Application.Features.Ingredients.Constants;
using OleAI.Backend.Application.Features.Ingredients.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Ingredients.Commands.DeleteIngredient
{
    public class DeleteIngredientCommandHandler : IRequestHandler<DeleteIngredientCommand, string>
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IUserIngredientRepository _userIngredientRepository;
        private readonly IngredientBusinessRules _businessRules;

        public DeleteIngredientCommandHandler(IIngredientRepository ingredientRepository, IUserIngredientRepository userIngredientRepository, IngredientBusinessRules businessRules)
        {
            _ingredientRepository = ingredientRepository;
            _userIngredientRepository = userIngredientRepository;
            _businessRules = businessRules;
        }

        public async Task<string> Handle(DeleteIngredientCommand command, CancellationToken cancellationToken)
        {
            var ingredient = await _ingredientRepository.GetAsync(i => i.Id == command.Id, cancellationToken: cancellationToken);
            _businessRules.IngredientMustExistWhenRequested(ingredient);

            QueryOptions<UserIngredient> query = new QueryOptions<UserIngredient>();
            query.Predicate = ui => ui.Id == command.Id;

            var isUsedInPantry = await _userIngredientRepository.GetListAsync(query, cancellationToken);

            if (isUsedInPantry.Items.Any())
            {
                await _ingredientRepository.DeleteAsync(ingredient!, permanent: false);
                return IngredientMessages.IngredientSoftDeletedDueToInventoryUsageMessage;
            }
            else
            {
                await _ingredientRepository.DeleteAsync(ingredient!, permanent: true);
                return IngredientMessages.IngredientDeletedMessage;
            }

        }
    }
}
