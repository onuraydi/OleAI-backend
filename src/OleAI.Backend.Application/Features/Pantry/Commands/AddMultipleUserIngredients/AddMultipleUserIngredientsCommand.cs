using MediatR;
using OleAI.Backend.Application.Features.Pantry.Dtos.Requests;
using OleAI.Backend.Application.Features.Pantry.Dtos.Responses;

namespace OleAI.Backend.Application.Features.Pantry.Commands.AddMultipleUserIngredients
{
    public class AddMultipleUserIngredientsCommand : IRequest<List<AddedUserIngredientResponse>>
    {
        public AddMultipleUserIngredientsRequest Request { get; set; } = default!;
    }
}
