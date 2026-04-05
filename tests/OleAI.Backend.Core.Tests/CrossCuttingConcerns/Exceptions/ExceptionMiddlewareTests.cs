using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Handlers;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.MiddleWares;
using Xunit;

namespace OleAI.Backend.Core.Tests.CrossCuttingConcerns.Exceptions;

public class ExceptionMiddlewareTests
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly Mock<HttpExceptionHandler> _handlerMock;
    private readonly Mock<ILogger<ExceptionMiddleware>> _loggerMock;
    private readonly ExceptionMiddleware _middleware;

    public ExceptionMiddlewareTests()
    {
        _nextMock = new Mock<RequestDelegate>();
        _handlerMock = new Mock<HttpExceptionHandler>();
        _loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
        _middleware = new ExceptionMiddleware(_nextMock.Object, _handlerMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task InvokeAsync_WhenNoException_ShouldCallNext()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        _nextMock.Verify(next => next(context), Times.Once);
        // We verify a virtual method that would be called inside HandleExceptionAsync
        _handlerMock.Verify(h => h.HandleException(It.IsAny<Exception>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_WhenExceptionThrown_ShouldHandleException()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var exception = new Exception("Test exception");
        _nextMock.Setup(next => next(It.IsAny<HttpContext>())).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        // HandleExceptionAsync(exception) calls HandleException(exception) which IS virtual.
        _handlerMock.Verify(h => h.HandleException(exception), Times.Once);
        Assert.Equal("application/json", context.Response.ContentType);
    }
}
