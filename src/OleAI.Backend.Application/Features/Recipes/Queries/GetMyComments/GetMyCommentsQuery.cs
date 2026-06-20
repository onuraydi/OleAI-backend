using MediatR;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Core.Repositories.Pagination;

namespace OleAI.Backend.Application.Features.Recipes.Queries.GetMyComments;

public class GetMyCommentsQuery : IRequest<Paginate<MyCommentResponse>>, IPageableRequest
{
    public int Page { get; set; } = 0;
    public int Size { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
}
