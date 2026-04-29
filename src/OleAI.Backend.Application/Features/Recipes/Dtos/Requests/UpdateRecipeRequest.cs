using Microsoft.AspNetCore.Http;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Recipes.Dtos.Requests
{
    public class UpdateRecipeRequest
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public RecipeVisibility Visibility { get; set; }
        public IFormFile? NewMainImage { get; set; }

    }
}
