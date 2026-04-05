using FluentAssertions;
using OleAI.Backend.Core.CrossCuttingConcerns.Mailing;

namespace OleAI.Backend.Core.Tests.CrossCuttingConcerns.Mailing;

public class MailSettingsTests
{
    [Fact]
    public void MailSettings_ShouldStoreConfigurationValues()
    {
        var settings = new MailSettings
        {
            Server = "smtp.example.com",
            Port = 587,
            SenderFullName = "OleAI",
            SenderEmail = "noreply@oleai.com",
            UserName = "smtp-user",
            Password = "smtp-pass"
        };

        settings.Server.Should().Be("smtp.example.com");
        settings.Port.Should().Be(587);
        settings.SenderFullName.Should().Be("OleAI");
        settings.SenderEmail.Should().Be("noreply@oleai.com");
        settings.UserName.Should().Be("smtp-user");
        settings.Password.Should().Be("smtp-pass");
    }
}
