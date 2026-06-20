using System.Collections.Generic;
using MediatR;
using OleAI.Backend.Application.Features.Pantry.Dtos.Responses;

namespace OleAI.Backend.Application.Features.Pantry.Queries.GetCategorizedPopularIngredients
{
    public class GetCategorizedPopularIngredientsQuery : IRequest<List<CategorizedPopularIngredientsResponse>>
    {
    }
}
