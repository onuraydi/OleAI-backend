using MediatR;
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.Recipes.Commands.RestoreRecipe;

public class RestoreRecipeCommandHandler : IRequestHandler<RestoreRecipeCommand, string>
{
    private readonly IRecipeRepository _recipeRepository;

    public RestoreRecipeCommandHandler(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }

    public async Task<string> Handle(RestoreRecipeCommand request, CancellationToken cancellationToken)
    {
        // DeletedDate null olmayan ve Id'si eşleşen tarifi bul.
        // EfRepositoryBase by default withDeleted=false ile çalışır, bu yüzden withDeleted: true geçmeliyiz.
        var recipe = await _recipeRepository.GetAsync(
            predicate: r => r.Id == request.Id,
            withDeleted: true,
            cancellationToken: cancellationToken);

        if (recipe == null)
            throw new Exception("Tarif bulunamadı."); // BusinessException da olabilir

        if (recipe.DeletedDate == null)
            throw new Exception("Tarif zaten silinmemiş.");

        // Geri yükle
        recipe.DeletedDate = null;
        await _recipeRepository.UpdateAsync(recipe);

        return RecipeMessages.RecipeRestored;
    }
}
