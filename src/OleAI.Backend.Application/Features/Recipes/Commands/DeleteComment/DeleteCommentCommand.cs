using MediatR;

namespace OleAI.Backend.Application.Features.Recipes.Commands.DeleteComment
{
    public class DeleteCommentCommand : IRequest<string>
    {
        public Guid CommentId { get; set; }
    }
}
