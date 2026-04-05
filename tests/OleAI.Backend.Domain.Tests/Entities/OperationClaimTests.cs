using FluentAssertions;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Domain.Tests.Entities;

public class OperationClaimTests
{
    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var claim = new OperationClaim
        {
            Id = Guid.NewGuid(),
            Name = "Admin"
        };

        claim.Name.Should().Be("Admin");
        claim.Id.Should().NotBe(Guid.Empty);
    }
}
