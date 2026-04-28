using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;

namespace OleAI.Backend.Application.Features.Recipes.Rules
{
    public class RecipeBusinessRules
    {
        private readonly IIngredientRepository _ingredientRepository;

        public RecipeBusinessRules(IIngredientRepository ingredientRepository)
        {
            _ingredientRepository = ingredientRepository;
        }

        /// <summary>
        /// Tarife eklenmek istenen malzemelerin veritabanında (sistemde) kayıtlı olup olmadığını kontrol eder.
        /// </summary>
        public async Task CheckIfIngredientsExistAsync(List<Guid> ingredientIds, CancellationToken cancellationToken)
        {
            if (!ingredientIds.Any()) return;

            // Gönderilen ID'lerin sistemde kaç tanesinin eşleştiğini buluyoruz
            var existingIngredientsCount = await _ingredientRepository.GetCountAsync(
                predicate: i => ingredientIds.Contains(i.Id),
                cancellationToken: cancellationToken);

            // Eğer sayılar uyuşmuyorsa, kullanıcı sistemde olmayan sahte bir ID göndermiştir
            if (existingIngredientsCount != ingredientIds.Count)
            {
                throw new BusinessException(RecipeMessages.InvalidIngredientsSelectedForRecipe);
            }
        }
    }
}
