using FluentAssertions;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Domain.Tests.Entities;

public class DietaryPreferenceExcludedCategoryTests
{
    [Fact]
    public void ObjectInitializer_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dietId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        var diet = new DietaryPreference();
        var category = new Category(Guid.NewGuid(), "Test Category");

        // Act
        var entity = new DietaryPreferenceExcludedCategory
        {
            DietaryPreferenceId = dietId,
            CategoryId = categoryId,
            DietaryPreference = diet,
            Category = category
        };

        // Assert
        entity.DietaryPreferenceId.Should().Be(dietId);
        entity.CategoryId.Should().Be(categoryId);
        entity.DietaryPreference.Should().Be(diet);
        entity.Category.Should().Be(category);
    }
}
