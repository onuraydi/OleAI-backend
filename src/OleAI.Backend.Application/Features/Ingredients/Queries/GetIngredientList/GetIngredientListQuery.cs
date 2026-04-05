using MediatR;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Responses;
using OleAI.Backend.Core.Repositories.Pagination;

namespace OleAI.Backend.Application.Features.Ingredients.Queries.GetIngredientList
{
    public class GetIngredientListQuery : IRequest<Paginate<IngredientResponse>>
    {
        public int Page { get; set; } = 0;
        public int Size { get; set; } = 10;
    }
}
