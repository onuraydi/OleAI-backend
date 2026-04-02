using OleAI.Backend.Application.Features.IngredientAliases.Constants;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.IngredientAliases.Rules
{
    public class IngredientAliasBusinessRules
    {
        private readonly IIngredientAliasRepository _aliasRepository;
        private readonly IIngredientRepository _ingredientRepository;

        public IngredientAliasBusinessRules(
            IIngredientAliasRepository aliasRepository,
            IIngredientRepository ingredientRepository)
        {
            _aliasRepository = aliasRepository;
            _ingredientRepository = ingredientRepository;
        }

        public void AliasMustExistWhenRequested(IngredientAlias? alias)
        {
            if (alias == null) throw new NotFoundException(IngredientAliasMessages.AliasNotFoundMessage);
        }

        public async Task MasterIngredientMustExist(Guid ingredientId)
        {
            var ingredient = await _ingredientRepository.GetAsync(i => i.Id == ingredientId);
            if (ingredient == null) throw new BusinessException(IngredientAliasMessages.MasterMaterialNotFoundMessage);
        }

        public async Task AliasNameCannotBeDuplicated(string aliasName)
        {
            var existingAlias = await _aliasRepository.GetAsync(a => a.AliasName.ToLower() == aliasName.ToLower());
            if (existingAlias != null)
                throw new BusinessException($"'{aliasName}' kelimesi zaten sistemde eşanlamlı olarak kayıtlı.");

            var existingMaster = await _ingredientRepository.GetAsync(i => i.Name.ToLower() == aliasName.ToLower());
            if (existingMaster != null)
                throw new BusinessException($"'{aliasName}' kelimesi sistemde zaten bir Ana Malzeme olarak bulunuyor, eşanlamlı olarak eklenemez.");
        }
    }
}
