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
    }
}
