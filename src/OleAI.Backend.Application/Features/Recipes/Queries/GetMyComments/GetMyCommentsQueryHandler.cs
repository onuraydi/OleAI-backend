using System.Security.Claims;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Pagination;

namespace OleAI.Backend.Application.Features.Recipes.Queries.GetMyComments;

public class GetMyCommentsQueryHandler : IRequestHandler<GetMyCommentsQuery, Paginate<MyCommentResponse>>
{
    private readonly IRecipeCommentRepository _recipeCommentRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetMyCommentsQueryHandler(IRecipeCommentRepository recipeCommentRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _recipeCommentRepository = recipeCommentRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Paginate<MyCommentResponse>> Handle(GetMyCommentsQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var options = new OleAI.Backend.Core.Repositories.Options.QueryOptions<Domain.Entities.RecipeComment>
        {
            Predicate = c => c.UserId == userId && c.DeletedDate == null &&
                             (string.IsNullOrEmpty(request.SearchTerm) ||
                              c.Content.ToLower().Contains(request.SearchTerm.ToLower()) ||
                              c.Recipe!.Title.ToLower().Contains(request.SearchTerm.ToLower())),
            Include = q => q.Include(c => c.Recipe!).ThenInclude(r => r.RecipeImages).Include(c => c.ParentComment!),
            OrderBy = q => request.SortBy switch
            {
                "Oldest" => q.OrderBy(c => c.CreatedDate),
                "Highest Rated" => q.OrderByDescending(c => c.Rating).ThenByDescending(c => c.CreatedDate),
                "Lowest Rated" => q.OrderBy(c => c.Rating).ThenByDescending(c => c.CreatedDate),
                _ => q.OrderByDescending(c => c.CreatedDate)
            },
            Index = request.Page,
            Size = request.Size,
            EnableTracking = false
        };

        var comments = await _recipeCommentRepository.GetListAsync(
            options,
            cancellationToken: cancellationToken);

        var response = _mapper.Map<Paginate<MyCommentResponse>>(comments);

        return response;
    }
}
