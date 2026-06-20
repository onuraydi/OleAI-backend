using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using OleAI.Backend.Persistence.Contexts;
using OleAI.Backend.Persistence.Repositories;
using Xunit;

namespace OleAI.Backend.Persistence.Tests.Repositories
{
    public class UserDeviceRepositoryTests
    {
        private static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldPersistUserDevice()
        {
            using var context = CreateContext();
            var repository = new UserDeviceRepository(context);

            var device = new UserDevice(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "token123",
                DevicePlatform.Android
            );

            await repository.AddAsync(device);

            var saved = await context.UserDevices.FindAsync(device.Id);
            saved.Should().NotBeNull();
            saved!.DeviceToken.Should().Be("token123");
            saved.Platform.Should().Be(DevicePlatform.Android);
        }
    }
}
