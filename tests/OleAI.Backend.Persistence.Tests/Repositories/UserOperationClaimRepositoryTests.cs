using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Persistence.Contexts;
using OleAI.Backend.Persistence.Repositories;

namespace OleAI.Backend.Persistence.Tests.Repositories;

public class UserOperationClaimRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistUserOperationClaim()
    {
        using var context = CreateContext();
        var repository = new UserOperationClaimRepository(context);

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "User",
            Email = "test.user@test.com",
            PasswordHash = new byte[] { 1 },
            PasswordSalt = new byte[] { 2 },
            Status = true
        };

        var operationClaim = new OperationClaim { Id = Guid.NewGuid(), Name = "Admin" };

        await context.Users.AddAsync(user);
        await context.OperationClaims.AddAsync(operationClaim);
        await context.SaveChangesAsync();

        var userOperationClaim = new UserOperationClaim
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            OperationClaimId = operationClaim.Id
        };

        await repository.AddAsync(userOperationClaim);

        var saved = await context.UserOperationClaims.FindAsync(userOperationClaim.Id);
        saved.Should().NotBeNull();
        saved!.UserId.Should().Be(user.Id);
        saved.OperationClaimId.Should().Be(operationClaim.Id);
    }
}
