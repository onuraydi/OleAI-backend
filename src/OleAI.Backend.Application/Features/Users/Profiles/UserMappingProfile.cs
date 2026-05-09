using AutoMapper;
using OleAI.Backend.Application.Features.Users.Dtos.Responses;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Users.Profiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserProfileResponse>()
                .ForMember(dest => dest.DietaryPreferences, opt => opt.MapFrom(
                    src => src.UserDietaryPreferences.Select(udp => new DietaryPreferenceItemResponse
                    {
                        Id = udp.DietaryPreferenceId,
                        Name = udp.DietaryPreference.Name
                    }).ToList()))
                .ForMember(dest => dest.Allergies, opt => opt.MapFrom(
                    src => src.UserAllergies.Select(ua => new AllergyRestrictionItemResponse
                    {
                        Id = ua.AllergyRestrictionId,
                        Name = ua.AllergyRestriction.Name
                    }).ToList()))
                .ForMember(dest => dest.DislikedIngredients, opt => opt.MapFrom(
                    src => src.UserDislikedIngredients.Select(udi => new DislikedIngredientItemResponse
                    {
                        Id = udi.IngredientId,
                        Name = udi.Ingredient.Name
                    }).ToList()));
        }
    }
}
