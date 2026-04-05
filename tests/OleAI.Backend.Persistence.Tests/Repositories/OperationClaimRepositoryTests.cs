using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Persistence.Contexts;
using OleAI.Backend.Persistence.Repositories;

namespace OleAI.Backend.Persistence.Tests.Repositories;

public class OperationClaimRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistOperationClaim()
    {
        using var context = CreateContext();
        var repository = new OperationClaimRepository(context);

        var claim = new OperationClaim { Id = Guid.NewGuid(), Name = "Admin" };
        await repository.AddAsync(claim);

        var saved = await context.OperationClaims.FindAsync(claim.Id);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("Admin");
    }
}
