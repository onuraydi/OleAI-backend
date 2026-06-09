using FluentAssertions;
using OleAI.Backend.Domain;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Domain.Tests.Entities;

public class RecipeTests
{
    [Fact]
    public void Create_ShouldInitializeCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var title = "Test Recipe";
        var description = "Test Description";
        var visibility = RecipeVisibility.Public;

        // Act
        var recipe = Recipe.Create(userId, title, description, visibility);

        // Assert
        recipe.UserId.Should().Be(userId);
        recipe.Title.Should().Be(title);
        recipe.Description.Should().Be(description);
        recipe.Visibility.Should().Be(visibility);
        recipe.Status.Should().Be(RecipeStatus.PendingLlmApproval);
        recipe.RecipeIngredients.Should().BeEmpty();
        recipe.RecipeSteps.Should().BeEmpty();
        recipe.RecipeImages.Should().BeEmpty();
    }

    [Fact]
    public void Create_WhenPrivate_ShouldSetStatusToPublished()
    {
        // Act
        var recipe = Recipe.Create(Guid.NewGuid(), "Title", "Desc", RecipeVisibility.Private);

        // Assert
        recipe.Status.Should().Be(RecipeStatus.Published);
    }

    [Fact]
    public void UpdateDetails_ShouldUpdateProperties()
    {
        // Arrange
        var recipe = Recipe.Create(Guid.NewGuid(), "Old Title", "Old Desc", RecipeVisibility.Public);

        // Act
        recipe.UpdateDetails("New Title", "New Desc", RecipeVisibility.Private);

        // Assert
        recipe.Title.Should().Be("New Title");
        recipe.Description.Should().Be("New Desc");
        recipe.Visibility.Should().Be(RecipeVisibility.Private);
        recipe.Status.Should().Be(RecipeStatus.Published);
    }

    [Fact]
    public void AddIngredient_ShouldAddIngredientToCollection()
    {
        // Arrange
        var recipe = Recipe.Create(Guid.NewGuid(), "Test", "Test", RecipeVisibility.Public);
        var ingredientId = Guid.NewGuid();

        // Act
        recipe.AddIngredient(ingredientId, null, 100, 1);
        recipe.AddIngredient(ingredientId, null, 100, 1);

        // Assert
        Assert.Equal(2, recipe.RecipeIngredients.Count);
    }

    [Fact]
    public void SyncIngredients_ShouldWorkCorrectly()
    {
        // Arrange
        var recipe = Recipe.Create(Guid.NewGuid(), "Test", "Test", RecipeVisibility.Public);
        var ing1 = Guid.NewGuid();
        var ing2 = Guid.NewGuid();
        recipe.AddIngredient(ing1, null, 100, 1);

        var incoming = new List<(Guid? IngredientId, Guid? IngredientRequestId, decimal Amount, int UnitId)>
        {
            (ing1, null, 200, 2),
            (ing2, null, 50, 1)
        };

        // Act
        recipe.SyncIngredients(incoming);

        // Assert
        recipe.RecipeIngredients.Should().HaveCount(2);
        recipe.RecipeIngredients.Should().ContainSingle(i => i.IngredientId == ing1 && i.Amount == 200 && i.UnitId == 2);
        recipe.RecipeIngredients.Should().ContainSingle(i => i.IngredientId == ing2 && i.Amount == 50 && i.UnitId == 1);
    }

    [Fact]
    public void SyncSteps_ShouldManageStepsAndReturnOrphanedImages()
    {
        // Arrange
        var recipe = Recipe.Create(Guid.NewGuid(), "Title", "Desc", RecipeVisibility.Public);
        recipe.AddStep(1, "Step 1");
        recipe.AddStep(2, "Step 2");
        var step2 = recipe.RecipeSteps.First(s => s.StepNumber == 2);
        recipe.AddImage("url", "public_id_2", false, step2.Id);

        var incoming = new List<(int StepNumber, string Instruction)>
        {
            (1, "Updated Step 1"),
            (3, "New Step 3")
        };

        // Act
        var orphanedImages = recipe.SyncSteps(incoming);

        // Assert
        recipe.RecipeSteps.Should().HaveCount(2);
        recipe.RecipeSteps.Should().ContainSingle(s => s.StepNumber == 1 && s.Instruction == "Updated Step 1");
        recipe.RecipeSteps.Should().ContainSingle(s => s.StepNumber == 3 && s.Instruction == "New Step 3");
        recipe.RecipeSteps.Should().NotContain(s => s.StepNumber == 2);

        orphanedImages.Should().Contain("public_id_2");
        recipe.RecipeImages.Should().BeEmpty();
    }

    [Fact]
    public void ReplaceImage_ShouldHandleMainImage()
    {
        // Arrange
        var recipe = Recipe.Create(Guid.NewGuid(), "Title", "Desc", RecipeVisibility.Public);
        recipe.AddImage("old_url", "old_public_id", true);

        // Act
        var oldPublicId = recipe.ReplaceImage("new_url", "new_public_id", true);

        // Assert
        oldPublicId.Should().Be("old_public_id");
        recipe.RecipeImages.Should().ContainSingle(i => i.IsMain && i.PublicId == "new_public_id");
    }

    [Fact]
    public void ApproveByAdmin_WhenPendingAdminApproval_ShouldSetToPublished()
    {
        // Arrange
        var recipe = Recipe.Create(Guid.NewGuid(), "Title", "Desc", RecipeVisibility.Public);
        recipe.PassLlmFilter(); // Status becomes PendingAdminApproval

        // Act
        recipe.ApproveByAdmin();

        // Assert
        recipe.Status.Should().Be(RecipeStatus.Published);
    }

    [Fact]
    public void ApproveByAdmin_WhenNotPendingAdminApproval_ShouldThrowException()
    {
        // Arrange
        var recipe = Recipe.Create(Guid.NewGuid(), "Title", "Desc", RecipeVisibility.Private); // Sets status to Published

        // Act
        Action act = () => recipe.ApproveByAdmin();

        // Assert
        act.Should().Throw<Exception>().WithMessage("Sadece onay bekleyen tarifler onaylanabilir.");
    }

    [Fact]
    public void Reject_ShouldSetStatusToRejected()
    {
        // Arrange
        var recipe = Recipe.Create(Guid.NewGuid(), "Title", "Desc", RecipeVisibility.Public);

        // Act
        recipe.Reject("Bad content");

        // Assert
        recipe.Status.Should().Be(RecipeStatus.Rejected);
    }
}
