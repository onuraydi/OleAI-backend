using FluentAssertions;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Domain.Tests.Entities;

public class RecipeCommentTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var recipeId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var content = "Great recipe!";
        var parentId = Guid.NewGuid();

        // Act
        var comment = new RecipeComment(id, recipeId, userId, content, parentId);

        // Assert
        comment.Id.Should().Be(id);
        comment.RecipeId.Should().Be(recipeId);
        comment.UserId.Should().Be(userId);
        comment.Content.Should().Be(content);
        comment.ParentCommentId.Should().Be(parentId);
        comment.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateContent_ShouldChangeContentAndSetUpdatedDate()
    {
        // Arrange
        var comment = new RecipeComment(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Old content", null, 5);

        // Act
        comment.UpdateContent("New content");

        // Assert
        comment.Content.Should().Be("New content");
        comment.UpdatedDate.Should().NotBeNull();
        comment.UpdatedDate.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
