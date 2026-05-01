using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Commands.UpdateComment;
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Application.Services.UserProfiles;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Commands.UpdateComment;

public class UpdateCommentCommandHandlerTests
{
    private readonly Mock<IRecipeCommentRepository> _mockCommentRepository;
    private readonly Mock<IUserProfileVectorService> _mockVectorService;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly RecipeBusinessRules _rules;
    private readonly UpdateCommentCommandHandler _handler;

    public UpdateCommentCommandHandlerTests()
    {
        _mockCommentRepository = new Mock<IRecipeCommentRepository>();
        _mockVectorService = new Mock<IUserProfileVectorService>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _rules = new RecipeBusinessRules(Mock.Of<IIngredientRepository>(), _mockCommentRepository.Object);

        _handler = new UpdateCommentCommandHandler(_mockCommentRepository.Object, _rules, _mockVectorService.Object, _mockHttpContextAccessor.Object);
    }

    [Fact]
    public async Task Handle_WhenOwner_ShouldUpdateAndReturnMessage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);
        var comment = new RecipeComment(Guid.NewGuid(), Guid.NewGuid(), userId, "Old");

        _mockCommentRepository.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<RecipeComment, bool>>>(), It.IsAny<Func<IQueryable<RecipeComment>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<RecipeComment, object>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);

        var command = new UpdateCommentCommand { CommentId = comment.Id, NewContent = "New" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(RecipeMessages.CommentUpdatedSuccessfullyMessage);
        comment.Content.Should().Be("New");
    }

    [Fact]
    public async Task Handle_WhenNotCommentOwner_ShouldThrowAuthorizationException()
    {
        // Arrange
        SetupUser(Guid.NewGuid());
        var ownerId = Guid.NewGuid();
        var comment = new RecipeComment(Guid.NewGuid(), Guid.NewGuid(), ownerId, "Old");

        _mockCommentRepository.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<RecipeComment, bool>>>(), It.IsAny<Func<IQueryable<RecipeComment>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<RecipeComment, object>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);

        var command = new UpdateCommentCommand { CommentId = comment.Id, NewContent = "New" };

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AuthorizationException>();
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
