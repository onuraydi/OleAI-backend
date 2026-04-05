using MediatR;
using OleAI.Backend.Application.Features.IngredientCategories.Dtos.Requests;

namespace OleAI.Backend.Application.Features.IngredientCategories.Commands.UpdateCategory
{
    public class UpdateCategoryCommand : IRequest<string>
    {
        public UpdateCategoryRequest Request { get; set; } = default!;
    }
}
