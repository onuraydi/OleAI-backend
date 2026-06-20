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
    public class ContactMessageRepositoryTests
    {
        private static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldPersistContactMessage()
        {
            using var context = CreateContext();
            var repository = new ContactMessageRepository(context);

            var message = new ContactMessage
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = "test@example.com",
                Department = "Support",
                Message = "Help me",
                IsRead = false
            };

            await repository.AddAsync(message);

            var saved = await context.ContactMessages.FindAsync(message.Id);
            saved.Should().NotBeNull();
            saved!.Email.Should().Be("test@example.com");
            saved.Name.Should().Be("Test User");
        }
    }
}
