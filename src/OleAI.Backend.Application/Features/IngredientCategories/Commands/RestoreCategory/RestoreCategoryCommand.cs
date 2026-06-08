using MediatR;

namespace OleAI.Backend.Application.Features.IngredientCategories.Commands.RestoreCategory
{
    public class RestoreCategoryCommand : IRequest<string>
    {
        public Guid Id { get; set; }
    }
}
