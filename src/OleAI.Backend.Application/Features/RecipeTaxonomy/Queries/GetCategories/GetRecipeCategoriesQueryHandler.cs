using MediatR;
using OleAI.Backend.Application.Features.RecipeTaxonomy.Dtos;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Domain.Entities;
namespace OleAI.Backend.Application.Features.RecipeTaxonomy.Queries.GetCategories;

public class GetRecipeCategoriesQueryHandler : IRequestHandler<GetRecipeCategoriesQuery, List<RecipeCategoryDto>>
{
    private readonly IRecipeCategoryRepository _repository;
    public GetRecipeCategoriesQueryHandler(IRecipeCategoryRepository repository) { _repository = repository; }
    public async Task<List<RecipeCategoryDto>> Handle(GetRecipeCategoriesQuery request, CancellationToken cancellationToken)
    {
        var query = new QueryOptions<RecipeCategory> { Size = 1000, EnableTracking = false };
        var list = await _repository.GetListAsync(query, cancellationToken);
        var items = list.Items.OrderBy(x => x.Name).ToList();
        return items.Select(x => new RecipeCategoryDto { Id = x.Id, Name = x.Name, IconName = x.IconName }).ToList();
    }
}
