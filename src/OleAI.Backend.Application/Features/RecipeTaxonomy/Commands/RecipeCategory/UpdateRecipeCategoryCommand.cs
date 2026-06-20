using MediatR;

namespace OleAI.Backend.Application.Features.RecipeTaxonomy.Commands.RecipeCategory;

public class UpdateRecipeCategoryCommand : IRequest<string>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string IconName { get; set; }
}
