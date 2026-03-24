using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Persistence;
using OleAI.Backend.Persistence.Contexts;
using Xunit;

namespace OleAI.Backend.Persistence.Tests.DependencyInjection;

public class PersistenceServiceRegistrationTests
{
    [Fact]
    public void AddPersistenceServices_ShouldRegisterRepositories()
    {
        // Arrange
        var services = new ServiceCollection();

        // Mocking IConfiguration correctly (avoiding extension methods in Setup)
        var mockConfig = new Mock<IConfiguration>();
        var mockSection = new Mock<IConfigurationSection>();

        mockConfig.Setup(x => x.GetSection("ConnectionStrings")).Returns(mockSection.Object);
        mockSection.Setup(x => x["OleAIConnectionString"]).Returns("Server=(localdb)\\mssqllocaldb;Database=OleAITest;Trusted_Connection=True;");

        // Act
        services.AddPersistanceServices(mockConfig.Object);
        var provider = services.BuildServiceProvider();

        // Assert
        Assert.NotNull(provider.GetService<AppDbContext>());
        Assert.NotNull(provider.GetService<IUserRepository>());
        Assert.NotNull(provider.GetService<IRefreshTokenRepository>());
        Assert.NotNull(provider.GetService<IOperationClaimRepository>());
        Assert.NotNull(provider.GetService<IUserOperationClaimRepository>());
    }
}
