using AutoMapper;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Requests;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Responses;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Ingredients.Profiles
{
    public class IngredientMappingProfile : Profile
    {
        public IngredientMappingProfile()
        {
            CreateMap<Ingredient, IngredientResponse>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : "Kategorisiz"));

            CreateMap<Paginate<Ingredient>, Paginate<IngredientResponse>>();

            CreateMap<CreateIngredientRequest, Ingredient>();
            CreateMap<UpdateIngredientRequest, Ingredient>();
            CreateMap<Ingredient, CreatedIngredientResponse>();
        }
    }
}
