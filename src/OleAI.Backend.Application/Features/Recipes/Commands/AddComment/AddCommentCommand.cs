using MediatR;

namespace OleAI.Backend.Application.Features.Recipes.Commands.AddComment;

public class AddCommentCommand : IRequest<string>
{
    public Guid RecipeId { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? ParentCommentId { get; set; }
    public int? Rating { get; set; }
}
