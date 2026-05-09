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

        var allergy = new AllergyRestriction();
        var category = new Category(Guid.NewGuid(), "Test Category");

        // Act
        var entity = new AllergyExcludedCategory
        {
            AllergyRestrictionId = allergyId,
            CategoryId = categoryId,
            AllergyRestriction = allergy,
            Category = category
        };

        // Assert
        entity.AllergyRestrictionId.Should().Be(allergyId);
        entity.CategoryId.Should().Be(categoryId);
        entity.AllergyRestriction.Should().Be(allergy);
        entity.Category.Should().Be(category);
    }
}
