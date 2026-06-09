using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Application.Services.UserProfiles;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Recipes.Commands.AddComment;

public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, string>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IRecipeCommentRepository _commentRepository;
    private readonly RecipeBusinessRules _recipeBusinessRules;
    private readonly IUserProfileVectorService _vectorService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AddCommentCommandHandler(
        IRecipeRepository recipeRepository,
        IRecipeCommentRepository commentRepository,
        RecipeBusinessRules recipeBusinessRules,
        IUserProfileVectorService vectorService,
        IHttpContextAccessor httpContextAccessor)
    {
        _recipeRepository = recipeRepository;
        _commentRepository = commentRepository;
        _recipeBusinessRules = recipeBusinessRules;
        _vectorService = vectorService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var recipe = await _recipeRepository.GetAsync(r => r.Id == request.RecipeId, cancellationToken: cancellationToken);
        _recipeBusinessRules.CheckIfRecipeIsInteractable(recipe, userId);

        await _recipeBusinessRules.CheckIfParentCommentExistsAsync(request.ParentCommentId, request.RecipeId, cancellationToken);

        var trimmedContent = request.Content.Trim();

        var comment = new RecipeComment(
            Guid.NewGuid(),
            request.RecipeId,
            userId,
            trimmedContent,
            request.ParentCommentId,
            request.Rating);

        await _commentRepository.AddAsync(comment);

        _ = _vectorService.UpdateProfileVectorAsync(userId, request.RecipeId, "Comment", CancellationToken.None);

        return request.ParentCommentId.HasValue ? "Cevabınız başarıyla eklendi." : "Yorumunuz başarıyla eklendi.";
    }
}
