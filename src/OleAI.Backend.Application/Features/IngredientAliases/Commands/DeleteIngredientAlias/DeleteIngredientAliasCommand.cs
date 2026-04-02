using MediatR;

namespace OleAI.Backend.Application.Features.IngredientAliases.Commands.DeleteIngredientAlias
{
    public class DeleteIngredientAliasCommand : IRequest<string>
    {
        public Guid Id { get; set; }
    }
}
