using FluentAssertions;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Domain.Tests.Entities;

public class UserDietaryPreferenceTests
{
    [Fact]
    public void ObjectInitializer_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var dietId = Guid.NewGuid();

        // Act
        var entity = new UserDietaryPreference
        {
            UserId = userId,
            DietaryPreferenceId = dietId
        };

        // Assert
        entity.UserId.Should().Be(userId);
        entity.DietaryPreferenceId.Should().Be(dietId);
    }
}
