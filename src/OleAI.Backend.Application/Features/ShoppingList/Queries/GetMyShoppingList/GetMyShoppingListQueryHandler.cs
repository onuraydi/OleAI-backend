using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.ShoppingList.Dtos.Responses;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.ShoppingList.Queries.GetMyShoppingList;

public class GetMyShoppingListQueryHandler
    : IRequestHandler<GetMyShoppingListQuery, List<ShoppingListItemResponse>>
{
    private readonly IShoppingListItemRepository _shoppingListItemRepository;
    private readonly IMapper _mapper;

    public GetMyShoppingListQueryHandler(
        IShoppingListItemRepository shoppingListItemRepository,
        IMapper mapper)
    {
        _shoppingListItemRepository = shoppingListItemRepository;
        _mapper = mapper;
    }

    public async Task<List<ShoppingListItemResponse>> Handle(
        GetMyShoppingListQuery query,
        CancellationToken cancellationToken)
    {
        var options = new QueryOptions<ShoppingListItem>
        {
            Predicate = s => s.UserId == query.UserId && !s.IsPurchased,
            Include = q => q.Include(s => s.Ingredient).ThenInclude(i => i!.Category),
            EnableTracking = false
        };

        var result = await _shoppingListItemRepository.GetListAsync(options, cancellationToken);

        return _mapper.Map<List<ShoppingListItemResponse>>(result.Items);
    }
}
