using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Persistence.Contexts;
using OleAI.Backend.Persistence.Repositories;

namespace OleAI.Backend.Persistence.Tests.Repositories;

public class UserRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistUser()
    {
        using var context = CreateContext();
        var repository = new UserRepository(context);

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Onur",
            LastName = "Aydin",
            Email = "onur@test.com",
            PasswordHash = new byte[] { 1 },
            PasswordSalt = new byte[] { 2 },
            IsEmailVerified = true
        };

        await repository.AddAsync(user);

        var saved = await context.Users.FindAsync(user.Id);
        saved.Should().NotBeNull();
        saved!.Email.Should().Be("onur@test.com");
    }

    [Fact]
    public async Task GetAsync_ShouldReturnUserByEmail()
    {
        using var context = CreateContext();
        var repository = new UserRepository(context);

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Ali",
            LastName = "Veli",
            Email = "ali@test.com",
            PasswordHash = new byte[] { 1 },
            PasswordSalt = new byte[] { 2 },
            IsEmailVerified = true
        };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var fetched = await repository.GetAsync(u => u.Email == "ali@test.com");

        fetched.Should().NotBeNull();
        fetched!.Id.Should().Be(user.Id);
    }
}
