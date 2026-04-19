using AutoMapper;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Requests;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Responses;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.IngredientAliases.Profiles
{
    public class IngredientAliasMappingProfile : Profile
    {
        public IngredientAliasMappingProfile()
        {
            CreateMap<IngredientAlias, IngredientAliasResponse>().ReverseMap();
            CreateMap<CreateIngredientAliasRequest, IngredientAlias>();
            CreateMap<UpdateIngredientAliasRequest, IngredientAlias>();
            CreateMap<IngredientAlias, CreatedIngredientAliasResponse>();
            CreateMap<IngredientAlias, UpdatedIngredientAliasResponse>();
        }
    }
}
