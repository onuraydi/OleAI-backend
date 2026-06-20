using MediatR;
using OleAI.Backend.Application.Features.Pantry.Commands.AddUserIngredient;
using OleAI.Backend.Application.Features.Pantry.Dtos.Responses;

namespace OleAI.Backend.Application.Features.Pantry.Commands.AddMultipleUserIngredients
{
    public class AddMultipleUserIngredientsCommandHandler : IRequestHandler<AddMultipleUserIngredientsCommand, List<AddedUserIngredientResponse>>
    {
        private readonly IMediator _mediator;

        public AddMultipleUserIngredientsCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<List<AddedUserIngredientResponse>> Handle(AddMultipleUserIngredientsCommand request, CancellationToken cancellationToken)
        {
            var responses = new List<AddedUserIngredientResponse>();

            foreach (var ingredientRequest in request.Request.Ingredients)
            {
                var addCommand = new AddUserIngredientCommand
                {
                    Request = ingredientRequest
                };

                // Handle each ingredient using the existing command handler logic
                var response = await _mediator.Send(addCommand, cancellationToken);
                responses.Add(response);
            }

            return responses;
        }
    }
}
