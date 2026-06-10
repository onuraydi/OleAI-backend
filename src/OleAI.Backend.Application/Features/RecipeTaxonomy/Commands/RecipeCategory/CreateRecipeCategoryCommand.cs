using MediatR;

namespace OleAI.Backend.Application.Features.RecipeTaxonomy.Commands.RecipeCategory;

public class CreateRecipeCategoryCommand : IRequest<string>
{
    public string Name { get; set; }
    public string IconName { get; set; }
}
