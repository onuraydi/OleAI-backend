using FluentAssertions;
using OleAI.Backend.Application.Features.IngredientCategories.Commands.RestoreCategory;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.IngredientCategories.Commands.RestoreCategory;

public class RestoreCategoryCommandTests
{
    [Fact]
    public void Properties_ShouldBeGettableAndSettable()
    {
        var dto = new RestoreCategoryCommand();
        var id = System.Guid.NewGuid();
        dto.Id = id;
        dto.Id.Should().Be(id);
    }
}
