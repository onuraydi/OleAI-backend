using AutoMapper;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Recipes.Profiles;

public class RecipeMappingProfile : Profile
{
    public RecipeMappingProfile()
    {
        CreateMap<Recipe, PendingAdminRecipeListResponse>();

        CreateMap<Paginate<Recipe>, Paginate<PendingAdminRecipeListResponse>>();

        CreateMap<Recipe, RecipeListItemResponse>()
            .ForMember(dest => dest.MainImageUrl, opt =>
                opt.MapFrom(src =>
                src.RecipeImages.FirstOrDefault(i => i.IsMain) != null
                ? src.RecipeImages.FirstOrDefault(i => i.IsMain)!.ImageUrl
                : string.Empty))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Paginate<Recipe>, Paginate<RecipeListItemResponse>>();

        CreateMap<Recipe, RecipeDetailResponse>();
        CreateMap<RecipeIngredient, RecipeIngredientResponse>();
        CreateMap<RecipeStep, RecipeStepResponse>();
        CreateMap<RecipeImage, RecipeImageResponse>();
    }
}
