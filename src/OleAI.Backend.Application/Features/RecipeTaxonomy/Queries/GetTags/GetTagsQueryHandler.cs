using MediatR;
using OleAI.Backend.Application.Features.RecipeTaxonomy.Dtos;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Domain.Entities;
namespace OleAI.Backend.Application.Features.RecipeTaxonomy.Queries.GetTags;

public class GetTagsQueryHandler : IRequestHandler<GetTagsQuery, List<TagDto>>
{
    private readonly ITagRepository _repository;
    public GetTagsQueryHandler(ITagRepository repository) { _repository = repository; }
    public async Task<List<TagDto>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        var query = new QueryOptions<Tag> { Size = 1000, EnableTracking = false };
        var list = await _repository.GetListAsync(query, cancellationToken);
        var items = list.Items.OrderBy(x => x.Name).ToList();
        return items.Select(x => new TagDto { Id = x.Id, Name = x.Name }).ToList();
    }
}
