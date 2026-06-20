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
    public class SystemLogRepositoryTests
    {
        private static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldPersistSystemLog()
        {
            using var context = CreateContext();
            var repository = new SystemLogRepository(context);

            var log = (SystemLog)Activator.CreateInstance(typeof(SystemLog), nonPublic: true)!;
            typeof(SystemLog).GetProperty("Id")?.SetValue(log, 1);
            typeof(SystemLog).GetProperty("Level")?.SetValue(log, "Info");
            typeof(SystemLog).GetProperty("Message")?.SetValue(log, "Test message");

            await repository.AddAsync(log);

            var saved = await context.SystemLogs.FindAsync(1);
            saved.Should().NotBeNull();
            saved!.Message.Should().Be("Test message");
            saved.Level.Should().Be("Info");
        }
    }
}
