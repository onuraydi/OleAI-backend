using MediatR;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.Recipes.Commands.RejectRecipe;

public class RejectRecipeCommandHandler : IRequestHandler<RejectRecipeCommand, string>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly RecipeBusinessRules _recipeBusinessRules;

    public RejectRecipeCommandHandler(IRecipeRepository recipeRepository, RecipeBusinessRules recipeBusinessRules)
    {
        _recipeRepository = recipeRepository;
        _recipeBusinessRules = recipeBusinessRules;
    }

    public async Task<string> Handle(RejectRecipeCommand request, CancellationToken cancellationToken)
    {
        var recipe = await _recipeRepository.GetAsync(r => r.Id == request.Id, cancellationToken: cancellationToken);

        _recipeBusinessRules.CheckIfRecipeExists(recipe);

        recipe!.Reject(request.Reason);

        await _recipeRepository.UpdateAsync(recipe);

        return $"Tarif reddedildi. Sebep: {request.Reason}";
    }
}
