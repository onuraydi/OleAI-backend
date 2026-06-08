using MediatR;
using OleAI.Backend.Application.Features.IngredientAliases.Constants;
using OleAI.Backend.Application.Features.IngredientAliases.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;

namespace OleAI.Backend.Application.Features.IngredientAliases.Commands.RestoreIngredientAlias
{
    public class RestoreIngredientAliasCommandHandler : IRequestHandler<RestoreIngredientAliasCommand, string>
    {
        private readonly IIngredientAliasRepository _ingredientAliasRepository;
        private readonly IngredientAliasBusinessRules _ingredientAliasBusinessRules;

        public RestoreIngredientAliasCommandHandler(IIngredientAliasRepository ingredientAliasRepository, IngredientAliasBusinessRules ingredientAliasBusinessRules)
        {
            _ingredientAliasRepository = ingredientAliasRepository;
            _ingredientAliasBusinessRules = ingredientAliasBusinessRules;
        }

        public async Task<string> Handle(RestoreIngredientAliasCommand request, CancellationToken cancellationToken)
        {
            var options = new QueryOptions<OleAI.Backend.Domain.Entities.IngredientAlias>
            {
                Predicate = ia => ia.Id == request.Id,
                WithDeleted = true
            };

            var result = await _ingredientAliasRepository.GetListAsync(options, cancellationToken: cancellationToken);
            var ingredientAlias = result.Items.FirstOrDefault();

            _ingredientAliasBusinessRules.AliasMustExistWhenRequested(ingredientAlias);

            ingredientAlias!.DeletedDate = null;
            await _ingredientAliasRepository.UpdateAsync(ingredientAlias!);

            return "Eşanlamlı kelime başarıyla geri getirildi.";
        }
    }
}
