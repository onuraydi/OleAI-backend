using MediatR;

namespace OleAI.Backend.Application.Features.Recipes.Commands.AnalyzeRecipeWithLlm
{
    public class AnalyzeRecipeWithLlmCommand : IRequest<string>
    {
        public Guid RecipeId { get; set; }
    }
}
