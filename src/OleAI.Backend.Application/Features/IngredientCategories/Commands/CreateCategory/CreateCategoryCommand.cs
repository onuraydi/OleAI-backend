using MediatR;
using OleAI.Backend.Application.Features.IngredientCategories.Dtos.Requests;
using OleAI.Backend.Application.Features.IngredientCategories.Dtos.Response;

namespace OleAI.Backend.Application.Features.IngredientCategories.Commands.CreateCategory
{
    public class CreateCategoryCommand : IRequest<CreatedCategoryResponse>
    {
        public CreateCategoryRequest Request { get; set; } = default!;
    }
}
