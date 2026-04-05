using FluentAssertions;
using OleAI.Backend.Application.Services.Llm;
using Xunit;

namespace OleAI.Backend.Application.Tests.Services.Llm;

public class OllamaSettingsTests
{
    [Fact]
    public void OllamaSettings_ShouldStoreConfiguredValues()
    {
        var settings = new OllamaSettings
        {
            BaseUrl = "http://localhost:11434",
            ModelName = "llama3.2",
            SystemPrompt = "system"
        };

        settings.BaseUrl.Should().Be("http://localhost:11434");
        settings.ModelName.Should().Be("llama3.2");
        settings.SystemPrompt.Should().Be("system");
    }
}
