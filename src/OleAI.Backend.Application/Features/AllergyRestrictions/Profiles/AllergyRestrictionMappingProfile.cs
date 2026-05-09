using AutoMapper;
using OleAI.Backend.Application.Features.AllergyRestrictions.Dtos.Responses;
using OleAI.Backend.Application.Features.IngredientCategories.Dtos.Response;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.AllergyRestrictions.Profiles;

public class AllergyRestrictionMappingProfile : Profile
{
    public AllergyRestrictionMappingProfile()
    {
        CreateMap<AllergyRestriction, AllergyRestrictionListResponse>()
            .ForMember(dest => dest.ExcludedIngredientCount, opt => opt.MapFrom(src => src.ExcludedIngredients.Count))
            .ForMember(dest => dest.ExcludedCategoryCount, opt => opt.MapFrom(src => src.ExcludedCategories.Count));

        CreateMap<AllergyRestriction, AllergyRestrictionDetailResponse>()
            .ForMember(dest => dest.ExcludedIngredients, opt => opt.MapFrom(src =>
                src.ExcludedIngredients.Select(ei => ei.Ingredient)))
            .ForMember(dest => dest.ExcludedCategories, opt => opt.MapFrom(src =>
                src.ExcludedCategories.Select(ec => ec.Category)));
    }
}
