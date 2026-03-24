using Microsoft.Extensions.Configuration;
using Moq;
using OleAI.Backend.Core.CrossCuttingConcerns.Mailing;
using Xunit;

namespace OleAI.Backend.Core.Tests.CrossCuttingConcerns.Exceptions;

public class SmtpMailManagerTests
{
    [Fact]
    public void Constructor_WithNullConfig_ShouldThrowException()
    {
        // Arrange
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(x => x.GetSection("MailSettings")).Returns((IConfigurationSection)null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new SmtpMailManager(mockConfig.Object));
    }

    [Fact]
    public void Constructor_WithValidConfig_ShouldNotThrow()
    {
        // Arrange
        var mockConfig = new Mock<IConfiguration>();
        var mockSection = new Mock<IConfigurationSection>();

        mockConfig.Setup(x => x.GetSection("MailSettings")).Returns(mockSection.Object);

        // Mocking the indexer and individual properties because .Get<T>() is an extension method
        mockSection.Setup(x => x["Server"]).Returns("smtp.test.com");
        mockSection.Setup(x => x["Port"]).Returns("587");
        mockSection.Setup(x => x["UserName"]).Returns("user");
        mockSection.Setup(x => x["Password"]).Returns("pass");
        mockSection.Setup(x => x["SenderEmail"]).Returns("no-reply@test.com");
        mockSection.Setup(x => x["SenderFullName"]).Returns("OleAI Test");

        // Note: SmtpMailManager uses .Get<MailSettings>() which internally uses bind logic.
        // It's better to use a real ConfigurationBuilder for this specific test to ensure .Get<T> works.
        var myConfiguration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"MailSettings:Server", "smtp.test.com"},
                {"MailSettings:Port", "587"},
                {"MailSettings:UserName", "user"},
                {"MailSettings:Password", "pass"},
                {"MailSettings:SenderEmail", "no-reply@test.com"},
                {"MailSettings:SenderFullName", "OleAI Test"}
            })
            .Build();

        // Act
        var manager = new SmtpMailManager(myConfiguration);

        // Assert
        Assert.NotNull(manager);
    }
}
