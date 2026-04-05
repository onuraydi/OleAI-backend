using MediatR;

namespace OleAI.Backend.Application.Features.Pantry.Commands.DeleteUserIngredient
{
    public class DeleteUserIngredientCommand : IRequest<string>
    {
        public Guid UserId { get; set; }
        public Guid Id { get; set; }
    }
}
