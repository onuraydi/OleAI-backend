using MediatR;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Requests;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Responses;

namespace OleAI.Backend.Application.Features.IngredientAliases.Commands.CreateIngredientAlias
{
    public class CreateIngredientAliasCommand : IRequest<CreatedIngredientAliasResponse>
    {
        public CreateIngredientAliasRequest Request { get; set; } = default!;
    }
}
