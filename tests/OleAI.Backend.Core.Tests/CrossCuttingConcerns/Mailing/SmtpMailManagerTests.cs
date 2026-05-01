using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using OleAI.Backend.Core.CrossCuttingConcerns.Mailing;
using Xunit;

namespace OleAI.Backend.Core.Tests.CrossCuttingConcerns.Mailing;

public class SmtpMailManagerTests
{
    [Fact]
    public void Constructor_WhenMailSettingsMissing_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.Setup(c => c.GetSection("MailSettings")).Returns(new Mock<IConfigurationSection>().Object);

        // Act
        Action act = () => new SmtpMailManager(mockConfiguration.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
