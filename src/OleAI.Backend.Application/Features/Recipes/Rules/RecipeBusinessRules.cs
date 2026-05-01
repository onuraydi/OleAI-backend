using System.Security.Claims;
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Recipes.Rules
{
    public class RecipeBusinessRules
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IRecipeCommentRepository _recipeCommentRepository;

        public RecipeBusinessRules(
            IIngredientRepository ingredientRepository,
            IRecipeCommentRepository recipeCommentRepository)
        {
            _ingredientRepository = ingredientRepository;
            _recipeCommentRepository = recipeCommentRepository;
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

        /// <summary>
        /// Tarifin veritabanında bulunup bulunmadığını kontrol eder. Yoksa 404 Not Found fırlatır.
        /// </summary>
        public void CheckIfRecipeExists(Recipe? recipe)
        {
            if (recipe == null)
            {
                throw new NotFoundException(RecipeMessages.RecipeNotFoundOrDeleted);
            }
        }

        /// <summary>
        /// İşlemi yapan kullanıcının, tarifin asıl sahibi olup olmadığını kontrol eder. Değilse 403 Forbidden/Unauthorized fırlatır.
        /// </summary>
        public void CheckIfUserIsOwner(Recipe recipe, Guid currentUserId)
        {
            if (recipe.UserId != currentUserId)
            {
                throw new AuthorizationException(RecipeMessages.UnauthorizedRecipeAccess);
            }
        }

        /// <summary>
        /// Kullanıcının Admin yetkisine sahip olup olmadığını kontrol eder.
        /// </summary>
        public void CheckIfAdmin(ClaimsPrincipal user)
        {
            if (!user.IsInRole("Admin"))
                throw new AuthorizationException(RecipeMessages.AdminRoleRequired);
        }
        /// <summary>
        /// Kural: Ya tarif Published & Public olmalı ya da sahibi mevcut kullanıcı olmalı
        /// </summary>
        public void CheckIfRecipeIsInteractable(Recipe? recipe, Guid currentUserId)
        {
            if (recipe == null) throw new NotFoundException(RecipeMessages.RecipeNotFound);

            bool isPublicAndPublished = recipe.Visibility == RecipeVisibility.Public && recipe.Status == RecipeStatus.Published;
            bool isOwner = recipe.UserId == currentUserId;

            if (!isPublicAndPublished && !isOwner)
                throw new AuthorizationException(RecipeMessages.RecipeNotOpenForInteractionYet);
        }

        /// <summary>
        /// Kural: Yorum cevaplamak için parent yorum aynı tarif altında olmak zorundadır
        /// </summary>
        public async Task CheckIfParentCommentExistsAsync(Guid? parentCommentId, Guid recipeId, CancellationToken cancellationToken)
        {
            if (!parentCommentId.HasValue) return;

            var exists = await _recipeCommentRepository.AnyAsync(
                predicate: c => c.Id == parentCommentId.Value && c.RecipeId == recipeId,
                cancellationToken: cancellationToken);

            if (!exists)
                throw new NotFoundException(RecipeMessages.TargetCommentMissing);
        }
    }
}
