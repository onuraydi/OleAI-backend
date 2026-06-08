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

        CreateMap<Recipe, RecipeDetailResponse>()
            .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.RecipeIngredients))
            .ForMember(dest => dest.Steps, opt => opt.MapFrom(src => src.RecipeSteps))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.RecipeImages));

        CreateMap<RecipeIngredient, RecipeIngredientResponse>()
            .ForMember(dest => dest.IngredientName, opt => opt.MapFrom(src => src.Ingredient.Name))
            .ForMember(dest => dest.UnitFamily, opt => opt.MapFrom(src => (int)src.Ingredient.UnitFamily));
        CreateMap<RecipeStep, RecipeStepResponse>();
        CreateMap<RecipeImage, RecipeImageResponse>();

        CreateMap<RecipeComment, RecipeCommentResponse>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
            .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => (string?)null))
            .ForMember(dest => dest.Replies, opt => opt.MapFrom(src => src.Replies));

        CreateMap<Paginate<RecipeComment>, Paginate<RecipeCommentResponse>>();
    }
}
