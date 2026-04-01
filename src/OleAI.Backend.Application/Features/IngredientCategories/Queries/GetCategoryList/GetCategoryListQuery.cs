using MediatR;
using OleAI.Backend.Application.Features.IngredientCategories.Dtos.Response;

namespace OleAI.Backend.Application.Features.IngredientCategories.Queries.GetCategoryList
{
    public class GetCategoryListQuery : IRequest<List<CategoryResponse>>
    {
    }
}
