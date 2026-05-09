using FluentAssertions;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Domain.Tests.Entities;

public class AllergyRestrictionTests
{
    [Fact]
    public void Constructor_WithParameters_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Gluten Allergy";
        var description = "No gluten allowed";

        // Act
        var allergy = new AllergyRestriction
        {
            Id = id,
            Name = name,
            Description = description
        };

        // Assert
        allergy.Id.Should().Be(id);
        allergy.Name.Should().Be(name);
        allergy.Description.Should().Be(description);
        allergy.ExcludedIngredients.Should().NotBeNull();
        allergy.ExcludedCategories.Should().NotBeNull();
        allergy.UserAllergies.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_Default_ShouldInitializeCollections()
    {
        // Act
        var allergy = new AllergyRestriction();

        // Assert
        allergy.ExcludedIngredients.Should().NotBeNull();
        allergy.ExcludedCategories.Should().NotBeNull();
        allergy.UserAllergies.Should().NotBeNull();
    }
}
