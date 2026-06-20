using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using OleAI.Backend.Persistence.Contexts;
using OleAI.Backend.Persistence.Repositories;

namespace OleAI.Backend.Persistence.Tests.Repositories;

public class ShoppingListItemRepositoryTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetListAsync_ShouldReturnOnlyUsersActiveItems()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var repository = new ShoppingListItemRepository(context);

        var targetUserId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var activeItem = ShoppingListItem.CreateStandard(targetUserId, Guid.NewGuid(), 2, MeasurementUnit.Piece);
        var purchasedItem = ShoppingListItem.CreateStandard(targetUserId, Guid.NewGuid(), 1, MeasurementUnit.Piece);
        purchasedItem.MarkAsPurchased();
        var otherUserItem = ShoppingListItem.CreateStandard(otherUserId, Guid.NewGuid(), 3, MeasurementUnit.Piece);

        await context.ShoppingListItems.AddRangeAsync(activeItem, purchasedItem, otherUserItem);
        await context.SaveChangesAsync();

        // Act
        var options = new QueryOptions<ShoppingListItem>
        {
            Predicate = i => i.UserId == targetUserId && !i.IsPurchased,
            EnableTracking = false
        };

        var result = await repository.GetListAsync(options);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items[0].Id.Should().Be(activeItem.Id);
    }

    [Fact]
    public async Task DeleteAsync_WithPermanentTrue_ShouldRemoveItemFromDatabase()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var repository = new ShoppingListItemRepository(context);

        var item = ShoppingListItem.CreateStandard(Guid.NewGuid(), Guid.NewGuid(), 1, MeasurementUnit.Piece);
        await context.ShoppingListItems.AddAsync(item);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(item, permanent: true);

        // Assert
        var deleted = await context.ShoppingListItems.FindAsync(item.Id);
        deleted.Should().BeNull();
    }
}

