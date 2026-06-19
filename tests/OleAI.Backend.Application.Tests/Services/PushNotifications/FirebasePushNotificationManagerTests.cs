using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using OleAI.Backend.Application.Services.PushNotifications;
using Xunit;

namespace OleAI.Backend.Application.Tests.Services.PushNotifications
{
    public class FirebasePushNotificationManagerTests
    {
        private readonly Mock<ILogger<FirebasePushNotificationManager>> _mockLogger;

        public FirebasePushNotificationManagerTests()
        {
            _mockLogger = new Mock<ILogger<FirebasePushNotificationManager>>();
        }

        [Fact]
        public async Task SendPushNotificationAsync_WhenFirebaseNotInitialized_ShouldCatchExceptionAndReturnFalse()
        {
            // Arrange
            var manager = new FirebasePushNotificationManager(_mockLogger.Object);

            // Act
            // In a unit test environment without Firebase initialized, DefaultInstance throws or is null
            var result = await manager.SendPushNotificationAsync("test-token", "Title", "Body");

            // Assert
            Assert.False(result);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task SendMulticastPushNotificationAsync_WhenFirebaseNotInitialized_ShouldCatchExceptionAndReturnFalse()
        {
            // Arrange
            var manager = new FirebasePushNotificationManager(_mockLogger.Object);
            var tokens = new List<string> { "token1", "token2" };

            // Act
            var result = await manager.SendMulticastPushNotificationAsync(tokens, "Title", "Body");

            // Assert
            Assert.False(result);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
