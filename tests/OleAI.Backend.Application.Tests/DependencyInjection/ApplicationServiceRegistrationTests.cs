using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OleAI.Backend.Application;
using OleAI.Backend.Application.Features.Auth.Rules;
using Xunit;

namespace OleAI.Backend.Application.Tests.DependencyInjection;

public class ApplicationServiceRegistrationTests
{
    [Fact]
    public void AddApplicationServices_ShouldRegisterExpectedServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockConfig = new Mock<IConfiguration>();
        var mockSection = new Mock<IConfigurationSection>();
        mockConfig.Setup(c => c.GetSection(It.IsAny<string>())).Returns(mockSection.Object);
        var mockRepo = new Mock<OleAI.Backend.Application.Services.Repositories.IUserRepository>();
        services.AddSingleton(mockRepo.Object);

        // Act
        services.AddLogging();
        services.AddApplicationServices(mockConfig.Object);
        var provider = services.BuildServiceProvider();

        // Assert
        Assert.NotNull(provider.GetService<IMediator>());
        Assert.NotNull(provider.GetService<IMapper>());
        Assert.NotNull(provider.GetService<AuthBusinessRules>());
    }
}
