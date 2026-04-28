using Microsoft.AspNetCore.Http;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Recipes.Dtos.Requests
{
    public class CreateRecipeRequest
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public RecipeVisibility Visibility { get; set; }

        public IFormFile MainImage { get; set; } = default!;
    }
}
