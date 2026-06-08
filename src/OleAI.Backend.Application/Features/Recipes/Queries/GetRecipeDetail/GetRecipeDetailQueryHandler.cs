using System.Security.Claims;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Recipes.Queries.GetRecipeDetail;

public class GetRecipeDetailQueryHandler : IRequestHandler<GetRecipeDetailQuery, RecipeDetailResponse>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IRecipeCommentRepository _recipeCommentRepository;
    private readonly IMapper _mapper;
    private readonly RecipeBusinessRules _recipeBusinessRules;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetRecipeDetailQueryHandler(
        IRecipeRepository recipeRepository,
        IRecipeCommentRepository recipeCommentRepository,
        IMapper mapper,
        RecipeBusinessRules recipeBusinessRules,
        IHttpContextAccessor httpContextAccessor)
    {
        _recipeRepository = recipeRepository;
        _recipeCommentRepository = recipeCommentRepository;
        _mapper = mapper;
        _recipeBusinessRules = recipeBusinessRules;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<RecipeDetailResponse> Handle(GetRecipeDetailQuery request, CancellationToken cancellationToken)
    {
        var recipe = await _recipeRepository.GetAsync(
            predicate: r => r.Id == request.Id,
            include: q => q.Include(r => r.RecipeIngredients)
                               .ThenInclude(ri => ri.Ingredient)
                           .Include(r => r.RecipeSteps)
                           .Include(r => r.RecipeImages)
                           .Include(r => r.Interactions),
            enableTracking: false,
            cancellationToken: cancellationToken);

        _recipeBusinessRules.CheckIfRecipeExists(recipe);


        if (!(recipe!.Visibility == RecipeVisibility.Public && recipe.Status == RecipeStatus.Published))
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) throw new UnauthorizedAccessException();

            _recipeBusinessRules.CheckIfUserIsOwner(recipe, Guid.Parse(userIdClaim));
        }

        var response = _mapper.Map<RecipeDetailResponse>(recipe);

        var totalComments = await _recipeCommentRepository.GetCountAsync(c => c.RecipeId == recipe.Id, cancellationToken: cancellationToken);

        response.Statistics = new RecipeStatisticsResponse
        {
            TotalComments = totalComments,
            TotalLikes = recipe.Interactions?.Count(i => i.IsLiked) ?? 0,
            TotalDislikes = recipe.Interactions?.Count(i => i.IsDisliked) ?? 0,
            TotalSaves = recipe.Interactions?.Count(i => i.IsSaved) ?? 0
        };

        var currentUserIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrWhiteSpace(currentUserIdClaim))
        {
            var currentUserId = Guid.Parse(currentUserIdClaim);
            var interaction = recipe.Interactions?.FirstOrDefault(i => i.UserId == currentUserId);

            if (interaction != null)
            {
                response.UserInteraction = new UserInteractionResponse
                {
                    IsLiked = interaction.IsLiked,
                    IsDisliked = interaction.IsDisliked,
                    IsSaved = interaction.IsSaved
                };
            }
        }

        return response;
    }
}
