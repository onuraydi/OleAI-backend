using FluentAssertions;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Domain.Tests.Entities;

public class UserAllergyTests
{
    [Fact]
    public void ObjectInitializer_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var allergyId = Guid.NewGuid();

        var user = new User();
        var allergy = new AllergyRestriction();

        // Act
        var entity = new UserAllergy
        {
            UserId = userId,
            AllergyRestrictionId = allergyId,
            User = user,
            AllergyRestriction = allergy
        };

        // Assert
        entity.UserId.Should().Be(userId);
        entity.AllergyRestrictionId.Should().Be(allergyId);
        entity.User.Should().Be(user);
        entity.AllergyRestriction.Should().Be(allergy);
    }
}
