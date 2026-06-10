using AutoMapper;
using OleAI.Backend.Application.Features.RecipeTaxonomy.Commands.RecipeCategory;
using OleAI.Backend.Application.Features.RecipeTaxonomy.Commands.Tag;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.RecipeTaxonomy.Profiles;

public class RecipeTaxonomyMappingProfile : Profile
{
    public RecipeTaxonomyMappingProfile()
    {
        CreateMap<CreateRecipeCategoryCommand, RecipeCategory>();
        CreateMap<UpdateRecipeCategoryCommand, RecipeCategory>();

        CreateMap<CreateTagCommand, Tag>();
        CreateMap<UpdateTagCommand, Tag>();
    }
}
