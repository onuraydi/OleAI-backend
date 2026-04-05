using FluentAssertions;
using OleAI.Backend.Core.Repositories;

namespace OleAI.Backend.Core.Tests.Repositories;

public class EntityTests
{
    private sealed class TestEntity : Entity<Guid>
    {
    }

    [Fact]
    public void Constructor_ShouldInitializeCreatedDate()
    {
        var before = DateTime.UtcNow;
        var entity = new TestEntity();
        var after = DateTime.UtcNow;

        entity.CreatedDate.Should().BeOnOrAfter(before);
        entity.CreatedDate.Should().BeOnOrBefore(after);
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var entity = new TestEntity
        {
            Id = Guid.NewGuid(),
            UpdatedDate = DateTime.UtcNow,
            DeletedDate = DateTime.UtcNow
        };

        entity.Id.Should().NotBe(Guid.Empty);
        entity.UpdatedDate.Should().NotBeNull();
        entity.DeletedDate.Should().NotBeNull();
    }
}
