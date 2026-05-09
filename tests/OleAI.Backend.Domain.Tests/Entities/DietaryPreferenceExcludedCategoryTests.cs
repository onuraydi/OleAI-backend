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

        // Act
        var entity = new DietaryPreferenceExcludedCategory
        {
            DietaryPreferenceId = dietId,
            CategoryId = categoryId
        };

        // Assert
        entity.DietaryPreferenceId.Should().Be(dietId);
        entity.CategoryId.Should().Be(categoryId);
    }
}
