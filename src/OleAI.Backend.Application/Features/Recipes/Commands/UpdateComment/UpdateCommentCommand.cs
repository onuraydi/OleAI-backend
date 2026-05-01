using MediatR;

namespace OleAI.Backend.Application.Features.Recipes.Commands.UpdateComment
{
    public class UpdateCommentCommand : IRequest<string>
    {
        public Guid CommentId { get; set; }
        public string NewContent { get; set; } = string.Empty;
    }
}
