using FluentAssertions;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Domain.Tests.Entities;

public class CategoryTests
{
    [Fact]
    public void Constructor_ShouldSetIdAndName()
    {
        var id = Guid.NewGuid();
        var category = new Category(id, "Bakliyat");

        category.Id.Should().Be(id);
        category.Name.Should().Be("Bakliyat");
    }

    [Fact]
    public void Constructor_ShouldInitializeIngredientsCollection()
    {
        var category = new Category(Guid.NewGuid(), "Sebze");

        category.Ingredients.Should().NotBeNull();
        category.Ingredients.Should().BeEmpty();
    }
}
