using AutoMapper;
using OleAI.Backend.Application.Features.DietaryPreferences.Dtos.Responses;
using OleAI.Backend.Application.Features.IngredientCategories.Dtos.Response;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Responses;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.DietaryPreferences.Profiles;

public class DietaryPreferenceMappingProfile : Profile
{
    public DietaryPreferenceMappingProfile()
    {
        CreateMap<DietaryPreference, DietaryPreferenceListResponse>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Desciption))
            .ForMember(dest => dest.ExcludedIngredientCount, opt => opt.MapFrom(src => src.ExcludedIngredients.Count))
            .ForMember(dest => dest.ExcludedCategoryCount, opt => opt.MapFrom(src => src.ExcludedCategories.Count));

        CreateMap<DietaryPreference, DietaryPreferenceDetailResponse>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Desciption))
            .ForMember(dest => dest.ExcludedIngredients, opt => opt.MapFrom(src =>
                src.ExcludedIngredients.Select(ei => ei.Ingredient)))
            .ForMember(dest => dest.ExcludedCategories, opt => opt.MapFrom(src =>
                src.ExcludedCategories.Select(ec => ec.Category)));
    }
}
