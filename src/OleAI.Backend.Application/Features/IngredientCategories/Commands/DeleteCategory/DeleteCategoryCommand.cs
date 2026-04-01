using MediatR;

namespace OleAI.Backend.Application.Features.IngredientCategories.Commands.DeleteCategory
{
    public class DeleteCategoryCommand : IRequest<string>
    {
        public Guid Id { get; set; }
    }
}
