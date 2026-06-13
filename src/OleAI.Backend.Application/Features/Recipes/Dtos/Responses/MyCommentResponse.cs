using System;

namespace OleAI.Backend.Application.Features.Recipes.Dtos.Responses;

public class MyCommentResponse
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public string RecipeName { get; set; } = string.Empty;
    public string? RecipeImageUrl { get; set; }
    public string Content { get; set; } = string.Empty;
    public int? Rating { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid? ParentCommentId { get; set; }
    public string? ParentCommentContent { get; set; }
}
