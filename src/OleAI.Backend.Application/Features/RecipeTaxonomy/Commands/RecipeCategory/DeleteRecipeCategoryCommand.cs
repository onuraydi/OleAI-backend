using MediatR;

namespace OleAI.Backend.Application.Features.RecipeTaxonomy.Commands.RecipeCategory;

public class DeleteRecipeCategoryCommand : IRequest<string>
{
    public int Id { get; set; }
}
