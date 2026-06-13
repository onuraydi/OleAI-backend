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
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Visibility, opt => opt.MapFrom(src => (int)src.Visibility))
            .ForMember(dest => dest.VisibilityName, opt => opt.MapFrom(src => src.Visibility.ToString()))
            .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.Comments != null && src.Comments.Any(c => c.Rating != null) ? src.Comments.Where(c => c.Rating != null).Average(c => c.Rating!.Value) : 0))
            .ForMember(dest => dest.TotalRatings, opt => opt.MapFrom(src => src.Comments != null ? src.Comments.Count(c => c.Rating != null) : 0))
            .ForMember(dest => dest.IsLiked, opt => opt.MapFrom((src, dest, destMember, context) =>
                context.TryGetItems(out var items) && items.ContainsKey("currentUserId") &&
                src.Interactions != null &&
                src.Interactions.Any(i => i.UserId == (Guid)items["currentUserId"] && i.IsLiked)))
            .ForMember(dest => dest.IsSaved, opt => opt.MapFrom((src, dest, destMember, context) =>
                context.TryGetItems(out var items) && items.ContainsKey("currentUserId") &&
                src.Interactions != null &&
                src.Interactions.Any(i => i.UserId == (Guid)items["currentUserId"] && i.IsSaved)));

        CreateMap<Paginate<Recipe>, Paginate<RecipeListItemResponse>>();

        CreateMap<Recipe, RecipeDetailResponse>()
            .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.RecipeIngredients))
            .ForMember(dest => dest.Steps, opt => opt.MapFrom(src => src.RecipeSteps))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.RecipeImages))
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.RecipeCategories))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.RecipeTags));

        CreateMap<RecipeCategoryMapping, RecipeCategoryDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RecipeCategory.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.RecipeCategory.Name))
            .ForMember(dest => dest.IconName, opt => opt.MapFrom(src => src.RecipeCategory.IconName));

        CreateMap<RecipeTag, TagDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Tag.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Tag.Name));

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

        CreateMap<RecipeComment, MyCommentResponse>()
            .ForMember(dest => dest.RecipeName, opt => opt.MapFrom(src => src.Recipe != null ? src.Recipe.Title : string.Empty))
            .ForMember(dest => dest.RecipeImageUrl, opt => opt.MapFrom(src => src.Recipe != null && src.Recipe.RecipeImages.FirstOrDefault(i => i.IsMain) != null ? src.Recipe.RecipeImages.FirstOrDefault(i => i.IsMain)!.ImageUrl : null))
            .ForMember(dest => dest.ParentCommentContent, opt => opt.MapFrom(src => src.ParentComment != null ? src.ParentComment.Content : null));

        CreateMap<Paginate<RecipeComment>, Paginate<MyCommentResponse>>();
    }
}
