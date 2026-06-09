using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Services.ImageStorage;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.Recipes.Commands.HardDeleteRecipe;

public class HardDeleteRecipeCommandHandler : IRequestHandler<HardDeleteRecipeCommand, string>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IImageService _imageService;

    public HardDeleteRecipeCommandHandler(IRecipeRepository recipeRepository, IImageService imageService)
    {
        _recipeRepository = recipeRepository;
        _imageService = imageService;
    }

    public async Task<string> Handle(HardDeleteRecipeCommand request, CancellationToken cancellationToken)
    {
        // DeletedDate fark etmeksizin getirelim, soft delete yapılmış da olabilir.
        var recipe = await _recipeRepository.GetAsync(
            predicate: r => r.Id == request.Id,
            include: q => q.Include(r => r.RecipeImages),
            withDeleted: true,
            cancellationToken: cancellationToken);

        if (recipe == null)
            throw new Exception("Tarif bulunamadı.");

        foreach (var image in recipe.RecipeImages)
        {
            await _imageService.DeleteAsync(image.PublicId, cancellationToken);
        }

        await _recipeRepository.DeleteAsync(recipe, permanent: true, cancellationToken: cancellationToken);

        return RecipeMessages.RecipeHardDeleted;
    }
}
