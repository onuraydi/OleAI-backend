using MediatR;
using OleAI.Backend.Application.Features.IngredientCategories.Dtos.Response;

namespace OleAI.Backend.Application.Features.IngredientCategories.Queries.GetCategoryById
{
    public class GetCategoryByIdQuery : IRequest<CategoryResponse>
    {
        public Guid Id { get; set; }
    }
}
