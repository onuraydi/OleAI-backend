using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.ImageStorage;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.Recipes.Commands.DeleteRecipe;

public class DeleteRecipeCommandHandler : IRequestHandler<DeleteRecipeCommand, string>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly RecipeBusinessRules _recipeBusinessRules;
    private readonly IImageService _imageService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DeleteRecipeCommandHandler(
        IRecipeRepository recipeRepository,
        RecipeBusinessRules recipeBusinessRules,
        IImageService imageService,
        IHttpContextAccessor httpContextAccessor)
    {
        _recipeRepository = recipeRepository;
        _recipeBusinessRules = recipeBusinessRules;
        _imageService = imageService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> Handle(DeleteRecipeCommand request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userId = Guid.Parse(userIdClaim!);

        var recipe = await _recipeRepository.GetAsync(
            predicate: r => r.Id == request.Id,
            include: q => q.Include(r => r.RecipeImages),
            cancellationToken: cancellationToken);

        _recipeBusinessRules.CheckIfRecipeExists(recipe);

        var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;
        if (!isAdmin)
        {
            _recipeBusinessRules.CheckIfUserIsOwner(recipe!, userId);
        }

        // Artık resimleri silmiyoruz çünkü Soft Delete yapıyoruz (Soft delete yapılan tarifin resimleri Cloudinary'de kalmalı)
        // Hard delete ayrı bir komutla yapılacak.
        await _recipeRepository.DeleteAsync(recipe, permanent: false);

        return RecipeMessages.RecipeDeletedWithRelatedContent;
    }
}
