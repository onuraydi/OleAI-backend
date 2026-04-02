using MediatR;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Responses;

namespace OleAI.Backend.Application.Features.IngredientAliases.Queries.GetAliasesByIngredientId
{
    public class GetAliasesByIngredientIdQuery : IRequest<List<IngredientAliasResponse>>
    {
        public Guid IngredientId { get; set; }
    }
}
