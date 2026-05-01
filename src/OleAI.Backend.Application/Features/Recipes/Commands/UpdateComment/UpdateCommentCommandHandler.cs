using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Application.Services.UserProfiles;

namespace OleAI.Backend.Application.Features.Recipes.Commands.UpdateComment
{
    public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, string>
    {
        private readonly IRecipeCommentRepository _commentRepository;
        private readonly RecipeBusinessRules _recipeBusinessRules;
        private readonly IUserProfileVectorService _userProfileVectorService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateCommentCommandHandler(IRecipeCommentRepository recipeCommentRepository, RecipeBusinessRules recipeBusinessRules, IUserProfileVectorService userProfileVectorService, IHttpContextAccessor httpContextAccessor)
        {
            _commentRepository = recipeCommentRepository;
            _recipeBusinessRules = recipeBusinessRules;
            _userProfileVectorService = userProfileVectorService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var comment = await _commentRepository.GetAsync(c => c.Id == request.CommentId, cancellationToken: cancellationToken);

            _recipeBusinessRules.CheckIfUserIsCommentOwner(comment, userId);

            comment!.UpdateContent(request.NewContent);
            await _commentRepository.UpdateAsync(comment);

            // AI'a güncelleme sinyali gönder
            _ = _userProfileVectorService.UpdateProfileVectorAsync(userId, comment.RecipeId, "Update_Comment", CancellationToken.None);

            return RecipeMessages.CommentUpdatedSuccessfullyMessage;
        }
    }
}
