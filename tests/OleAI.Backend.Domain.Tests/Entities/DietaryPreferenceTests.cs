using FluentAssertions;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Domain.Tests.Entities;

public class DietaryPreferenceTests
{
    [Fact]
    public void Constructor_WithParameters_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Vegan";
        var description = "No animal products";

        // Act
        var preference = new DietaryPreference
        {
            Id = id,
            Name = name
        };

        // Assert
        preference.Id.Should().Be(id);
        preference.Name.Should().Be(name);
        preference.ExcludedIngredients.Should().NotBeNull();
        preference.ExcludedCategories.Should().NotBeNull();
        preference.UserDietaryPreferences.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_Default_ShouldInitializeCollections()
    {
        // Act
        var preference = new DietaryPreference();

        // Assert
        preference.ExcludedIngredients.Should().NotBeNull();
        preference.ExcludedCategories.Should().NotBeNull();
        preference.UserDietaryPreferences.Should().NotBeNull();
    }
}
