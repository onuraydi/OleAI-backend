using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Commands.DeleteComment;
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Application.Services.UserProfiles;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Commands.DeleteComment;

public class DeleteCommentCommandHandlerTests
{
    private readonly Mock<IRecipeCommentRepository> _mockCommentRepository;
    private readonly Mock<IUserProfileVectorService> _mockVectorService;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly RecipeBusinessRules _rules;
    private readonly DeleteCommentCommandHandler _handler;

    public DeleteCommentCommandHandlerTests()
    {
        _mockCommentRepository = new Mock<IRecipeCommentRepository>();
        _mockVectorService = new Mock<IUserProfileVectorService>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _rules = new RecipeBusinessRules(Mock.Of<IIngredientRepository>(), _mockCommentRepository.Object);

        _handler = new DeleteCommentCommandHandler(
            _mockCommentRepository.Object,
            _rules,
            _mockVectorService.Object,
            _mockHttpContextAccessor.Object);
    }

    [Fact]
    public async Task Handle_WhenOwner_ShouldDeleteAndReturnMessage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);

        var comment = new RecipeComment(Guid.NewGuid(), Guid.NewGuid(), userId, "Bye", null, 5);
        _mockCommentRepository.Setup(r => r.GetAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<RecipeComment, bool>>>(),
                It.IsAny<System.Func<System.Linq.IQueryable<RecipeComment>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<RecipeComment, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);

        var command = new DeleteCommentCommand { CommentId = comment.Id };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(RecipeMessages.CommentDeletedSuccessfullyMessage);
        _mockCommentRepository.Verify(r => r.DeleteAsync(comment, It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private void SetupUser(Guid userId)
    {
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = principal };
        _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(httpContext);
    }
}
