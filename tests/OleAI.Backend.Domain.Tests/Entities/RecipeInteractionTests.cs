using FluentAssertions;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Domain.Tests.Entities;

public class RecipeInteractionTests
{
    [Fact]
    public void Constructor_ShouldSetIds()
    {
        // Arrange
        var id = Guid.NewGuid();
        var recipeId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var interaction = new RecipeInteraction(id, recipeId, userId);

        // Assert
        interaction.Id.Should().Be(id);
        interaction.RecipeId.Should().Be(recipeId);
        interaction.UserId.Should().Be(userId);
        interaction.IsLiked.Should().BeFalse();
        interaction.IsDisliked.Should().BeFalse();
        interaction.IsSaved.Should().BeFalse();
    }

    [Fact]
    public void ToggleLike_ShouldWorkAndResetDislike()
    {
        // Arrange
        var interaction = new RecipeInteraction(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        interaction.ToggleDislike(); // Set disliked true

        // Act
        interaction.ToggleLike();

        // Assert
        interaction.IsLiked.Should().BeTrue();
        interaction.IsDisliked.Should().BeFalse();

        // Act - Toggle off
        interaction.ToggleLike();
        interaction.IsLiked.Should().BeFalse();
    }

    [Fact]
    public void ToggleDislike_ShouldWorkAndResetLike()
    {
        // Arrange
        var interaction = new RecipeInteraction(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        interaction.ToggleLike(); // Set liked true

        // Act
        interaction.ToggleDislike();

        // Assert
        interaction.IsDisliked.Should().BeTrue();
        interaction.IsLiked.Should().BeFalse();

        // Act - Toggle off
        interaction.ToggleDislike();
        interaction.IsDisliked.Should().BeFalse();
    }

    [Fact]
    public void ToggleSave_ShouldWork()
    {
        // Arrange
        var interaction = new RecipeInteraction(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        // Act
        interaction.ToggleSave();
        interaction.IsSaved.Should().BeTrue();

        interaction.ToggleSave();
        interaction.IsSaved.Should().BeFalse();
    }
}
