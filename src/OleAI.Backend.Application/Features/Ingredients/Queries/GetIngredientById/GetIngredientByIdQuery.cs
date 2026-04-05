using MediatR;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Responses;

namespace OleAI.Backend.Application.Features.Ingredients.Queries.GetIngredientById
{
    public class GetIngredientByIdQuery : IRequest<IngredientResponse>
    {
        public Guid Id { get; set; }
    }
}
