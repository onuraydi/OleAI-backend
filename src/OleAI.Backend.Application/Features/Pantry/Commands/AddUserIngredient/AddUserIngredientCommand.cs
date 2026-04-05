using MediatR;
using OleAI.Backend.Application.Features.Pantry.Dtos.Requests;
using OleAI.Backend.Application.Features.Pantry.Dtos.Responses;

namespace OleAI.Backend.Application.Features.Pantry.Commands.AddUserIngredient
{
    public class AddUserIngredientCommand : IRequest<AddedUserIngredientResponse>
    {
        public AddUserIngredientRequest Request { get; set; } = default!;
    }
}
