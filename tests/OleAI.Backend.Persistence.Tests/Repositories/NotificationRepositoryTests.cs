using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Persistence.Contexts;
using OleAI.Backend.Persistence.Repositories;
using Xunit;

namespace OleAI.Backend.Persistence.Tests.Repositories
{
    public class NotificationRepositoryTests
    {
        private static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldPersistNotification()
        {
            using var context = CreateContext();
            var repository = new NotificationRepository(context);

            var notification = new Notification(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Test Title",
                "Test Body",
                OleAI.Backend.Domain.Enums.NotificationType.System
            );

            await repository.AddAsync(notification);

            var saved = await context.Notifications.FindAsync(notification.Id);
            saved.Should().NotBeNull();
            saved!.Title.Should().Be("Test Title");
            saved.Message.Should().Be("Test Body");
        }
    }
}
