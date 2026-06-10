using Microsoft.AspNetCore.Http;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Recipes.Dtos.Requests
{
    public class UpdateRecipeRequest
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public RecipeDifficulty Difficulty { get; set; }
        public int Servings { get; set; }
        public int PrepTime { get; set; }
        public int CookTime { get; set; }
        public RecipeVisibility Visibility { get; set; }
        public List<int> CategoryIds { get; set; } = new();
        public List<int> TagIds { get; set; } = new();
        public IFormFile? NewMainImage { get; set; }

    }
}
