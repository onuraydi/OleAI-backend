using FluentAssertions;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Domain.Tests.Entities;

public class AllergyExcludedCategoryTests
{
    [Fact]
    public void ObjectInitializer_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var allergyId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        // Act
        var entity = new AllergyExcludedCategory
        {
            AllergyRestrictionId = allergyId,
            CategoryId = categoryId
        };

        // Assert
        entity.AllergyRestrictionId.Should().Be(allergyId);
        entity.CategoryId.Should().Be(categoryId);
    }
}
