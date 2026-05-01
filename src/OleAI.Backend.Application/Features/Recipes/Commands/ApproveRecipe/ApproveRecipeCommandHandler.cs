using MediatR;
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.Recipes.Commands.ApproveRecipe;

public class ApproveRecipeCommandHandler : IRequestHandler<ApproveRecipeCommand, string>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly RecipeBusinessRules _recipeBusinessRules;

    public ApproveRecipeCommandHandler(IRecipeRepository recipeRepository, RecipeBusinessRules recipeBusinessRules)
    {
        _recipeRepository = recipeRepository;
        _recipeBusinessRules = recipeBusinessRules;
    }

    public async Task<string> Handle(ApproveRecipeCommand request, CancellationToken cancellationToken)
    {
        var recipe = await _recipeRepository.GetAsync(r => r.Id == request.Id, cancellationToken: cancellationToken);

        _recipeBusinessRules.CheckIfRecipeExists(recipe);

        recipe!.ApproveByAdmin();

        await _recipeRepository.UpdateAsync(recipe);

        return RecipeMessages.RecipeApprovedAndPublished;
    }
}
