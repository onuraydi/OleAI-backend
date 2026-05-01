using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Application.Services.UserProfiles;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Recipes.Commands.ToggleInteraction;

public class ToggleInteractionCommandHandler : IRequestHandler<ToggleInteractionCommand, string>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IRecipeInteractionRepository _interactionRepository;
    private readonly RecipeBusinessRules _recipeBusinessRules;
    private readonly IUserProfileVectorService _vectorService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ToggleInteractionCommandHandler(
        IRecipeRepository recipeRepository,
        IRecipeInteractionRepository interactionRepository,
        RecipeBusinessRules recipeBusinessRules,
        IUserProfileVectorService vectorService,
        IHttpContextAccessor httpContextAccessor)
    {
        _recipeRepository = recipeRepository;
        _interactionRepository = interactionRepository;
        _recipeBusinessRules = recipeBusinessRules;
        _vectorService = vectorService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> Handle(ToggleInteractionCommand request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var recipe = await _recipeRepository.GetAsync(r => r.Id == request.RecipeId, cancellationToken: cancellationToken);
        _recipeBusinessRules.CheckIfRecipeIsInteractable(recipe, userId);

        ValidateInteractionType(request.InteractionType);

        var interaction = await _interactionRepository.GetAsync(
            predicate: i => i.RecipeId == request.RecipeId && i.UserId == userId,
            cancellationToken: cancellationToken);

        if (interaction == null)
        {
            interaction = new RecipeInteraction(Guid.NewGuid(), request.RecipeId, userId);
            await _interactionRepository.AddAsync(interaction);
        }

        ApplyInteractionToggle(interaction, request.InteractionType);

        await _interactionRepository.UpdateAsync(interaction);

        _ = _vectorService.UpdateProfileVectorAsync(userId, request.RecipeId, request.InteractionType, CancellationToken.None);

        return $"{request.InteractionType} işlemi başarıyla gerçekleştirildi.";
    }

    private static void ValidateInteractionType(string interactionType)
    {
        var validTypes = new[] { "Like", "Dislike", "Save" };
        if (!validTypes.Contains(interactionType, StringComparer.OrdinalIgnoreCase))
        {
            throw new BusinessException("Geçersiz etkileşim türü. Geçerli değerler: Like, Dislike, Save");
        }
    }

    private static void ApplyInteractionToggle(RecipeInteraction interaction, string interactionType)
    {
        switch (interactionType.ToUpperInvariant())
        {
            case "LIKE":
                interaction.ToggleLike();
                break;
            case "DISLIKE":
                interaction.ToggleDislike();
                break;
            case "SAVE":
                interaction.ToggleSave();
                break;
            default:
                throw new InvalidOperationException("Beklenmeyen etkileşim türü.");
        }
    }
}
