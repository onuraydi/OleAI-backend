using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;

namespace OleAI.Backend.Application.Features.Users.Dtos.Responses
{
    public class RecipeRecommendationDto
    {
        public RecipeListItemResponse Recipe { get; set; } = null!;
        public double MatchScore { get; set; }
        public bool IsFallback { get; set; }
    }
}
