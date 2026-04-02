using MediatR;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Responses;

namespace OleAI.Backend.Application.Features.IngredientAliases.Queries.GetIngredientAliasList
{
    public class GetIngredientAliasListQuery : IRequest<List<IngredientAliasResponse>>
    {
    }
}
