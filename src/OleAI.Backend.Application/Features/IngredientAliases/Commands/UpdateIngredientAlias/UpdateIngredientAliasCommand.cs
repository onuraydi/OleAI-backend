using MediatR;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Requests;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Responses;

namespace OleAI.Backend.Application.Features.IngredientAliases.Commands.UpdateIngredientAlias
{
    public class UpdateIngredientAliasCommand : IRequest<UpdatedIngredientAliasResponse>
    {
        public UpdateIngredientAliasRequest Requesst { get; set; } = default!;
    }
}
