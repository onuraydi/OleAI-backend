using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using OleAI.Backend.Persistence.Contexts;
using OleAI.Backend.Persistence.Repositories;

namespace OleAI.Backend.Persistence.Tests.Repositories
{
    public class IngredientRequestRepositoryTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task UpdateAsync_Should_Successfully_Save_LlmAnalysisResult()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new IngredientRequestRepository(context);

            var request = new IngredientRequest(Guid.NewGuid(), Guid.NewGuid(), "Pudrs Şelkeri");
            await context.IngredientRequests.AddAsync(request);
            await context.SaveChangesAsync();

            // Simüle edilmiş LLM JSON yanıtı
            string mockLlmJson = "{\"IsNew\": false, \"SuggestedAliasFor\": \"Şeker\"}";

            // Act - Domain Behavior (İş Kuralı) çağrımı
            request.UpdateLlmAnalysis(mockLlmJson);
            await repository.UpdateAsync(request);

            // Assert
            var updatedRequest = await context.IngredientRequests.FindAsync(request.Id);

            updatedRequest.Should().NotBeNull();
            updatedRequest!.Status.Should().Be(IngredientRequestStatus.Pending, "Statü henüz admin tarafından değiştirilmedi.");
            updatedRequest.LlmAnalysisResult.Should().Be(mockLlmJson);
        }
    }
}
