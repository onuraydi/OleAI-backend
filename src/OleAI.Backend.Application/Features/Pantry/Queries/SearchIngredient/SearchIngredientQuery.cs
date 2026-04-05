using MediatR;
using OleAI.Backend.Application.Features.Pantry.Dtos.Responses;

namespace OleAI.Backend.Application.Features.Pantry.Queries.SearchIngredient
{
    public class SearchIngredientQuery : IRequest<List<IngredientSearchResultResponse>>
    {
        public string Keyword { get; set; } = string.Empty;
    }
}
