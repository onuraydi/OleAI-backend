using AutoMapper;
using OleAI.Backend.Application.Features.ShoppingList.Dtos.Responses;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.ShoppingList.Profiles;

public class ShoppingListMappingProfile : Profile
{
    public ShoppingListMappingProfile()
    {
        CreateMap<ShoppingListItem, ShoppingListItemResponse>()
            .ForMember(dest => dest.IngredientName,
                opt => opt.MapFrom(src => src.Ingredient != null ? src.Ingredient.Name : string.Empty))
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src =>
                    src.Ingredient != null && src.Ingredient.Category != null
                        ? src.Ingredient.Category.Name
                        : string.Empty))
            .ForMember(dest => dest.CategoryIconName,
                opt => opt.MapFrom(src =>
                    src.Ingredient != null && src.Ingredient.Category != null
                        ? src.Ingredient.Category.IconName
                        : "fastfood"));
    }
}
