using MediatR;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Responses;

namespace OleAI.Backend.Application.Features.IngredientAliases.Queries.GetIngredientAliasById
{
    public class GetIngredientAliasByIdQuery : IRequest<IngredientAliasResponse>
    {
        public Guid Id { get; set; }
    }
}
