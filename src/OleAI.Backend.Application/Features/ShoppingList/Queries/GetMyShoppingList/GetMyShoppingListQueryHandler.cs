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
            Include = q => q
                .Include(s => s.Ingredient).ThenInclude(i => i!.Category)
                .Include(s => s.IngredientRequest).ThenInclude(ir => ir!.Category),
            EnableTracking = false
        };

        var result = await _shoppingListItemRepository.GetListAsync(options, cancellationToken);

        var responseList = new List<ShoppingListItemResponse>();
        foreach (var item in result.Items)
        {
            var dto = _mapper.Map<ShoppingListItemResponse>(item);

            if (item.IngredientId.HasValue && item.Ingredient != null)
            {
                dto.IngredientName = item.Ingredient.Name;
                dto.CategoryName = item.Ingredient.Category?.Name ?? "DİĞER";
                dto.CategoryIconName = item.Ingredient.Category?.IconName ?? "fastfood";
            }
            else if (item.IngredientRequestId.HasValue && item.IngredientRequest != null)
            {
                dto.IngredientName = item.IngredientRequest.RequestedName;
                dto.CategoryName = item.IngredientRequest.Category?.Name ?? "DİĞER";
                dto.CategoryIconName = item.IngredientRequest.Category?.IconName ?? "fastfood";
            }

            responseList.Add(dto);
        }

        return responseList;
    }
}
