using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using OleAI.Backend.Application.Features.Ingredients.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Application.Services.UserProfiles;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Users.Commands.UpdateUserDislikedIngredients;

public class UpdateUserDislikedIngredientsCommandHandler : IRequestHandler<UpdateUserDislikedIngredientsCommand, string>
{
    private readonly IUserDislikedIngredientRepository _userDislikedIngredientRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICacheService _cacheService;
    private readonly IUserProfileVectorService _userProfileVectorService;
    private readonly IngredientBusinessRules _ingredientRules;

    public UpdateUserDislikedIngredientsCommandHandler(
        IUserDislikedIngredientRepository userDislikedIngredientRepository,
        IHttpContextAccessor httpContextAccessor,
        ICacheService cacheService,
        IUserProfileVectorService userProfileVectorService,
        IngredientBusinessRules ingredientRules)
    {
        _userDislikedIngredientRepository = userDislikedIngredientRepository;
        _httpContextAccessor = httpContextAccessor;
        _cacheService = cacheService;
        _userProfileVectorService = userProfileVectorService;
        _ingredientRules = ingredientRules;
    }

    public async Task<string> Handle(UpdateUserDislikedIngredientsCommand request, CancellationToken cancellationToken)
    {
        var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            throw new AuthorizationException("Kullanıcı kimliği doğrulanamadı.");

        if (request.IngredientIds.Count > 0)
        {
            var distinctIds = request.IngredientIds.Distinct().ToList();
            await _ingredientRules.CheckIfIngredientsExistAsync(distinctIds, cancellationToken);
        }

        // Upsert pattern: Hard delete existing and insert new
        await _userDislikedIngredientRepository.ExecuteDeleteAsync(
            predicate: udi => udi.UserId == userId,
            cancellationToken: cancellationToken);

        if (request.IngredientIds.Count > 0)
        {
            var newDislikes = request.IngredientIds.Distinct().Select(ingredientId => new UserDislikedIngredient
            {
                UserId = userId,
                IngredientId = ingredientId
            });

            await _userDislikedIngredientRepository.AddRangeAsync(newDislikes, cancellationToken);
        }

        // Cache invalidation
        await _cacheService.RemoveAsync($"user-profile:{userId}");

        // Vektör senkronizasyonu
        _ = _userProfileVectorService.UpdateDietaryProfileAsync(userId, cancellationToken);

        return "Sevilmeyen malzemeleriniz başarıyla güncellendi.";
    }
}
