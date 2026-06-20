using MediatR;

namespace OleAI.Backend.Application.Features.IngredientAliases.Commands.RestoreIngredientAlias
{
    public class RestoreIngredientAliasCommand : IRequest<string>
    {
        public Guid Id { get; set; }
    }
}
