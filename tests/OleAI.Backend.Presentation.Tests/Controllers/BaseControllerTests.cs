using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OleAI.Backend.Presentation.Controllers;
using Xunit;

namespace OleAI.Backend.Presentation.Tests.Controllers;

public class TestController : BaseController
{
    public IMediator GetMediator() => Mediator;
}

public class BaseControllerTests
{
    [Fact]
    public void Mediator_Property_Should_Resolve_From_RequestServices()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(mockMediator.Object);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var httpContext = new DefaultHttpContext { RequestServices = serviceProvider };
        var controller = new TestController
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext }
        };

        // Act
        var result = controller.GetMediator();

        // Assert
        Assert.Equal(mockMediator.Object, result);
    }
}
