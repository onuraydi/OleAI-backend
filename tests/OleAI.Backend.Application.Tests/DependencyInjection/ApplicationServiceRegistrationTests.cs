using AutoMapper;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OleAI.Backend.Application;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Features.IngredientCategories.Rules;
using OleAI.Backend.Application.Features.Ingredients.Rules;
using OleAI.Backend.Application.Features.Pantry.Rules;
using OleAI.Backend.Application.Services.Llm;
using Xunit;

namespace OleAI.Backend.Application.Tests.DependencyInjection;

public class ApplicationServiceRegistrationTests
{
    [Fact]
    public void AddApplicationServices_ShouldRegisterExpectedServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["OllamaSettings:BaseUrl"] = "http://localhost:11434",
                ["OllamaSettings:ModelName"] = "llama3.2",
                ["OllamaSettings:SystemPrompt"] = "system"
            })
            .Build();
        var mockRepo = new Mock<OleAI.Backend.Application.Services.Repositories.IUserRepository>();
        var mockIngredientRepo = new Mock<OleAI.Backend.Application.Services.Repositories.IIngredientRepository>();
        var mockCategoryRepo = new Mock<OleAI.Backend.Application.Services.Repositories.ICategoryRepository>();
        var mockUserIngredientRepo = new Mock<OleAI.Backend.Application.Services.Repositories.IUserIngredientRepository>();
        var mockIngredientRequestRepo = new Mock<OleAI.Backend.Application.Services.Repositories.IIngredientRequestRepository>();
        services.AddSingleton(mockRepo.Object);
        services.AddSingleton(mockIngredientRepo.Object);
        services.AddSingleton(mockCategoryRepo.Object);
        services.AddSingleton(mockUserIngredientRepo.Object);
        services.AddSingleton(mockIngredientRequestRepo.Object);

        // Act
        services.AddLogging();
        services.AddApplicationServices(configuration);
        var provider = services.BuildServiceProvider();

        // Assert
        Assert.NotNull(provider.GetService<IMediator>());
        Assert.NotNull(provider.GetService<IMapper>());
        Assert.NotNull(provider.GetService<AuthBusinessRules>());
        Assert.NotNull(provider.GetService<PantryBusinessRules>());
        Assert.NotNull(provider.GetService<CategoryBusinessRules>());
        Assert.NotNull(provider.GetService<IngredientBusinessRules>());
        Assert.NotNull(provider.GetService<ILocalLlmService>());
    }
}
