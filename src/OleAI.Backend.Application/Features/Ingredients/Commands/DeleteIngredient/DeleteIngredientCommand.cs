using MediatR;

namespace OleAI.Backend.Application.Features.Ingredients.Commands.DeleteIngredient
{
    public class DeleteIngredientCommand : IRequest<string>
    {
        public Guid Id { get; set; }
    }
}
