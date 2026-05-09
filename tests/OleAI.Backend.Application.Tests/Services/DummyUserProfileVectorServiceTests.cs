using FluentAssertions;
using OleAI.Backend.Application.Services.UserProfiles;
using Xunit;

namespace OleAI.Backend.Application.Tests.Services;

public class DummyUserProfileVectorServiceTests
{
    [Fact]
    public async Task UpdateProfileVectorAsync_ShouldReturnCompletedTask()
    {
        // Arrange
        var service = new DummyUserProfileVectorService();
        var userId = Guid.NewGuid();

        // Act
        Func<Task> action = async () => await service.UpdateProfileVectorAsync(userId, Guid.NewGuid(), "test", default);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task UpdateDietaryProfileAsync_ShouldReturnCompletedTask()
    {
        // Arrange
        var service = new DummyUserProfileVectorService();
        var userId = Guid.NewGuid();

        // Act
        Func<Task> action = async () => await service.UpdateDietaryProfileAsync(userId, default);

        // Assert
        await action.Should().NotThrowAsync();
    }
}
