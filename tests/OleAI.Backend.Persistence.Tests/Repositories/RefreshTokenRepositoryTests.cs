using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Persistence.Contexts;
using OleAI.Backend.Persistence.Repositories;
using Xunit;

namespace OleAI.Backend.Persistence.Tests.Repositories;

public class RefreshTokenRepositoryTests
{
    private readonly DbContextOptions<AppDbContext> _dbContextOptions;

    public RefreshTokenRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task AddAsync_ShouldAddRefreshTokenToDatabase()
    {
        // Arrange
        using var context = new AppDbContext(_dbContextOptions);
        var repository = new RefreshTokenRepository(context);

        var token = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Token = "sample-refresh-token",
            Expires = DateTime.UtcNow.AddDays(7),
            CreatedByIp = "127.0.0.1"
        };

        // Act
        var addedToken = await repository.AddAsync(token);

        // Assert
        addedToken.Should().NotBeNull();
        addedToken.Id.Should().Be(token.Id);

        var tokenInDb = await context.RefreshTokens.FirstOrDefaultAsync(t => t.Id == token.Id);
        tokenInDb.Should().NotBeNull();
        tokenInDb!.Token.Should().Be("sample-refresh-token");
    }

    [Fact]
    public async Task GetAsync_ShouldReturnRefreshTokenByTokenValue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tokenValue = "existing-token-value";

        using (var initialContext = new AppDbContext(_dbContextOptions))
        {
            var initialToken = new RefreshToken
            {
                Id = id,
                UserId = Guid.NewGuid(),
                Token = tokenValue,
                Expires = DateTime.UtcNow.AddDays(7),
                CreatedByIp = "127.0.0.1"
            };

            await initialContext.RefreshTokens.AddAsync(initialToken);
            await initialContext.SaveChangesAsync();
        }

        using var context = new AppDbContext(_dbContextOptions);
        var repository = new RefreshTokenRepository(context);

        // Act
        var retrievedToken = await repository.GetAsync(t => t.Token == tokenValue);

        // Assert
        retrievedToken.Should().NotBeNull();
        retrievedToken!.Id.Should().Be(id);
        retrievedToken.Token.Should().Be(tokenValue);
    }
}
