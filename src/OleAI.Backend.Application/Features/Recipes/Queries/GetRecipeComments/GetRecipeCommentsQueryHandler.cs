using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Pagination;

namespace OleAI.Backend.Application.Features.Recipes.Queries.GetRecipeComments;

public class GetRecipeCommentsQueryHandler : IRequestHandler<GetRecipeCommentsQuery, Paginate<RecipeCommentResponse>>
{
    private readonly IRecipeCommentRepository _recipeCommentRepository;
    private readonly IMapper _mapper;

    public GetRecipeCommentsQueryHandler(IRecipeCommentRepository recipeCommentRepository, IMapper mapper)
    {
        _recipeCommentRepository = recipeCommentRepository;
        _mapper = mapper;
    }

    public async Task<Paginate<RecipeCommentResponse>> Handle(GetRecipeCommentsQuery request, CancellationToken cancellationToken)
    {
        var options = new OleAI.Backend.Core.Repositories.Options.QueryOptions<Domain.Entities.RecipeComment>
        {
            Predicate = c => c.RecipeId == request.RecipeId && c.ParentCommentId == null && c.DeletedDate == null,
            Include = q => q.Include(c => c.User)
                           .Include(c => c.Replies.Where(r => r.DeletedDate == null)).ThenInclude(r => r.User),
            OrderBy = q => q.OrderByDescending(c => c.CreatedDate),
            Index = request.Page,
            Size = request.Size,
            EnableTracking = false
        };

        var comments = await _recipeCommentRepository.GetListAsync(
            options,
            cancellationToken: cancellationToken);

        var response = _mapper.Map<Paginate<RecipeCommentResponse>>(comments);

        // Sort replies by oldest first or newest first. Usually newest first.
        foreach (var comment in response.Items)
        {
            if (comment.Replies != null && comment.Replies.Any())
            {
                comment.Replies = comment.Replies.OrderByDescending(r => r.CreatedDate).ToList();
            }
        }

        return response;
    }
}
