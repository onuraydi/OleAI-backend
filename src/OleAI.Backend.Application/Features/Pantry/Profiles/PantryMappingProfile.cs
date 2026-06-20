using AutoMapper;
using OleAI.Backend.Application.Features.Pantry.Dtos;
using OleAI.Backend.Application.Features.Pantry.Dtos.Responses;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Pantry.Profiles
{
    public class PantryMappingProfile : Profile
    {
        public PantryMappingProfile()
        {
            CreateMap<Ingredient, IngredientCacheModel>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : "Kategorisiz"))
                .ForMember(dest => dest.CategoryIconName, opt => opt.MapFrom(src => src.Category != null ? src.Category.IconName : "fastfood"));

            CreateMap<UserIngredient, PantryItemResponse>()
                .ForMember(dest => dest.MasterIngredientId, opt => opt.MapFrom(src => src.IngredientId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                    src.IngredientAlias != null ? src.IngredientAlias.AliasName :
                    (src.Ingredient != null ? src.Ingredient.Name :
                    (src.IngredientRequest != null ? src.IngredientRequest.RequestedName : "Bilinmeyen Ürün"))))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Ingredient != null && src.Ingredient.Category != null ? src.Ingredient.Category.Name : "Kategorisiz"))
                .ForMember(dest => dest.CategoryIconName, opt => opt.MapFrom(src => src.Ingredient != null && src.Ingredient.Category != null ? src.Ingredient.Category.IconName : "fastfood"))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit.ToString()))
                .ForMember(dest => dest.IsPendingApproval, opt => opt.MapFrom(src => src.IngredientRequestId != null && src.IngredientId == null));

            CreateMap<IngredientRequest, ResolvedRequestsResponse>();
        }
    }
}
