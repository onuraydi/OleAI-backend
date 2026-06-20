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
    public class TagRepositoryTests
    {
        private static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldPersistTag()
        {
            using var context = CreateContext();
            var repository = new TagRepository(context);

            var tag = new Tag(1, "Vegan");

            await repository.AddAsync(tag);

            var saved = await context.Tags.FindAsync(1);
            saved.Should().NotBeNull();
            saved!.Name.Should().Be("Vegan");
        }
    }
}
