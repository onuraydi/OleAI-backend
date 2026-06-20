using FluentAssertions;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Domain.Tests.Entities;

public class UserDeviceTests
{
    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var device = new UserDevice(id, userId, "token", DevicePlatform.IOS);

        device.Id.Should().Be(id);
        device.UserId.Should().Be(userId);
        device.DeviceToken.Should().Be("token");
        device.Platform.Should().Be(DevicePlatform.IOS);
        device.LastActiveAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateLastActiveAt_ShouldUpdate()
    {
        var device = new UserDevice(Guid.NewGuid(), Guid.NewGuid(), "token", DevicePlatform.IOS);
        var original = device.LastActiveAt;

        System.Threading.Thread.Sleep(10);
        device.UpdateLastActiveAt();

        device.LastActiveAt.Should().BeAfter(original);
    }
}
