using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Persistence.Contexts;
using Xunit;

namespace OleAI.Backend.Persistence.Tests.Contexts;

public class AppDbContextTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public void ShouldExposeExpectedDbSets()
    {
        using var context = CreateContext();

        context.Users.Should().NotBeNull();
        context.OperationClaims.Should().NotBeNull();
        context.UserOperationClaims.Should().NotBeNull();
        context.RefreshTokens.Should().NotBeNull();
        context.Categories.Should().NotBeNull();
        context.Ingredients.Should().NotBeNull();
        context.IngredientAliases.Should().NotBeNull();
        context.UserIngredients.Should().NotBeNull();
        context.IngredientRequests.Should().NotBeNull();
    }

    [Fact]
    public void OnModelCreating_ShouldApplyEntityConfigurations()
    {
        using var context = CreateContext();

        var categoryEntity = context.Model.FindEntityType(typeof(Category));
        var userEntity = context.Model.FindEntityType(typeof(User));
        var operationClaimEntity = context.Model.FindEntityType(typeof(OperationClaim));

        categoryEntity.Should().NotBeNull();
        userEntity.Should().NotBeNull();
        operationClaimEntity.Should().NotBeNull();

        categoryEntity!.GetTableName().Should().Be("Categories");
        userEntity!.GetTableName().Should().Be("Users");
        operationClaimEntity!.GetTableName().Should().Be("OperationClaims");
    }
}
