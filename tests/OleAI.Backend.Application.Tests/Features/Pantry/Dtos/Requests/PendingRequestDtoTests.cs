using FluentAssertions;
using OleAI.Backend.Application.Features.Pantry.Dtos.Requests;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Pantry.Dtos.Requests;

public class PendingRequestDtoTests
{
    [Fact]
    public void PendingRequestDto_ShouldStoreValues()
    {
        var now = DateTime.UtcNow;
        var dto = new PendingRequestDto
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            RequestedName = "Domat",
            LlmAnalysisResult = "{\"IsNew\":false}",
            CreatedDate = now
        };

        dto.RequestedName.Should().Be("Domat");
        dto.LlmAnalysisResult.Should().Be("{\"IsNew\":false}");
        dto.CreatedDate.Should().Be(now);
    }
}
