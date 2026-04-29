using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.Llm;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Recipes.Commands.ChangeRecipeVisibility;

public class ChangeRecipeVisibilityCommandHandler : IRequestHandler<ChangeRecipeVisibilityCommand, string>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly RecipeBusinessRules _recipeBusinessRules;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILocalLlmService _localLlmService;

    public ChangeRecipeVisibilityCommandHandler(
        IRecipeRepository recipeRepository,
        RecipeBusinessRules recipeBusinessRules,
        IHttpContextAccessor httpContextAccessor,
        ILocalLlmService localLlmService)
    {
        _recipeRepository = recipeRepository;
        _recipeBusinessRules = recipeBusinessRules;
        _httpContextAccessor = httpContextAccessor;
        _localLlmService = localLlmService;
    }

    public async Task<string> Handle(ChangeRecipeVisibilityCommand request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) throw new UnauthorizedAccessException();
        var userId = Guid.Parse(userIdClaim);

        var recipe = await _recipeRepository.GetAsync(
            predicate: r => r.Id == request.Id,
            cancellationToken: cancellationToken);

        _recipeBusinessRules.CheckIfRecipeExists(recipe);
        _recipeBusinessRules.CheckIfUserIsOwner(recipe!, userId);

        if (recipe!.Visibility == request.Visibility)
            return RecipeMessages.VisibilityStatusIsAlreadySame;

        recipe.SetVisibilityAndStatus(request.Visibility);

        if (recipe.Visibility == RecipeVisibility.Public)
        {
            bool isClean = await _localLlmService.CheckRecipeContentForSpamAsync(recipe.Title, recipe.Description, cancellationToken);
            if (isClean)
                recipe.PassLlmFilter();
            else
                recipe.Reject(RecipeMessages.RecipePublishingRejectedDueToContent);
        }

        await _recipeRepository.UpdateAsync(recipe);

        if (recipe.Status == RecipeStatus.Rejected)
            return RecipeMessages.VisibilityUpdateFailedStandards;

        return request.Visibility == RecipeVisibility.Public
            ? RecipeMessages.RecipePublishedAndQueuedForReview
            : RecipeMessages.RecipeVisibilityChangedToPrivate;
    }
}
