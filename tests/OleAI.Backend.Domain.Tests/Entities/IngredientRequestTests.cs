using FluentAssertions;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Domain.Tests.Entities
{
    public class IngredientRequestTests
    {
        [Fact]
        public void Request_Should_BeInitialized_AsPending()
        {
            // Arrange & Act
            var request = new IngredientRequest(Guid.NewGuid(), Guid.NewGuid(), "Pudrs Şelkeri");

            // Assert
            request.Status.Should().Be(IngredientRequestStatus.Pending);
        }

        [Fact]
        public void Reject_Should_ChangeStatus_And_SetAdminNote()
        {
            // Arrange
            var request = new IngredientRequest(Guid.NewGuid(), Guid.NewGuid(), "asdfg");

            // Act
            request.Reject("Anlamsız kelime girişi.");

            // Assert
            request.Status.Should().Be(IngredientRequestStatus.Rejected);
            request.AdminNote.Should().Be("Anlamsız kelime girişi.");
        }

        [Fact]
        public void ResolveAsMerged_Should_ChangeStatus_And_SetAdminNote()
        {
            // Arrange
            var request = new IngredientRequest(Guid.NewGuid(), Guid.NewGuid(), "Domat");

            // Act
            request.ResolveAsMerged("Alias olarak eklendi.");

            // Assert
            request.Status.Should().Be(IngredientRequestStatus.MergedWithAlias);
            request.AdminNote.Should().Be("Alias olarak eklendi.");
        }
    }
}
