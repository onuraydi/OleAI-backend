using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Recipes.Dtos.Responses
{
    public class RecipeDetailResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public RecipeVisibility Visibility { get; set; }
        public RecipeStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public List<RecipeIngredientResponse> Ingredients { get; set; } = new();
        public List<RecipeStepResponse> Steps { get; set; } = new();
        public List<RecipeImageResponse> Images { get; set; } = new();

        public RecipeStatisticsResponse Statistics { get; set; } = new();
        public UserInteractionResponse? UserInteraction { get; set; }
        public BasicUserInfo? Author { get; set; }
    }

    public class RecipeStatisticsResponse
    {
        public int TotalComments { get; set; }
        public int TotalLikes { get; set; }
        public int TotalDislikes { get; set; }
        public int TotalSaves { get; set; }
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
    }

    public class UserInteractionResponse
    {
        public bool IsLiked { get; set; }
        public bool IsDisliked { get; set; }
        public bool IsSaved { get; set; }
    }

    public class BasicUserInfo
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
    }

    public class RecipeCommentResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public string Content { get; set; } = string.Empty;
        public Guid? ParentCommentId { get; set; }
        public int? Rating { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<RecipeCommentResponse> Replies { get; set; } = new();
    }
}
