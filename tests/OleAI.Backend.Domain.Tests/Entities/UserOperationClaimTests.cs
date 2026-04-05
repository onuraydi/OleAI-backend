using FluentAssertions;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Domain.Tests.Entities;

public class UserOperationClaimTests
{
    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var userId = Guid.NewGuid();
        var operationClaimId = Guid.NewGuid();

        var userOperationClaim = new UserOperationClaim
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            OperationClaimId = operationClaimId
        };

        userOperationClaim.UserId.Should().Be(userId);
        userOperationClaim.OperationClaimId.Should().Be(operationClaimId);
    }
}
