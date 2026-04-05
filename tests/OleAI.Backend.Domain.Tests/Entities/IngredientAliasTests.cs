using FluentAssertions;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Domain.Tests.Entities;

public class IngredientAliasTests
{
    [Fact]
    public void Constructor_ShouldSetAllProperties()
    {
        var id = Guid.NewGuid();
        var ingredientId = Guid.NewGuid();

        var alias = new IngredientAlias(id, ingredientId, "Domat");

        alias.Id.Should().Be(id);
        alias.IngredientId.Should().Be(ingredientId);
        alias.AliasName.Should().Be("Domat");
    }
}
