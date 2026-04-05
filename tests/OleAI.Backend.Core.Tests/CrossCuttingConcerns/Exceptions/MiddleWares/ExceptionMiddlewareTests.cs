using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Handlers;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.MiddleWares;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;

namespace OleAI.Backend.Core.Tests.CrossCuttingConcerns.Exceptions.MiddleWares;

public class ExceptionMiddlewareTests
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly HttpExceptionHandler _httpExceptionHandler;
    private readonly Mock<ILogger<ExceptionMiddleware>> _loggerMock;
    private readonly ExceptionMiddleware _middleware;
    private readonly DefaultHttpContext _httpContext;

    public ExceptionMiddlewareTests()
    {
        _nextMock = new Mock<RequestDelegate>();
        _httpExceptionHandler = new HttpExceptionHandler();
        _loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
        _middleware = new ExceptionMiddleware(_nextMock.Object, _httpExceptionHandler, _loggerMock.Object);

        _httpContext = new DefaultHttpContext();
        _httpContext.Response.Body = new MemoryStream();
    }

    [Fact]
    public async Task InvokeAsync_WhenNoException_ShouldCallNextDelegate()
    {
        // Arrange
        _nextMock.Setup(next => next(_httpContext)).Returns(Task.CompletedTask);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _nextMock.Verify(next => next(_httpContext), Times.Once);
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task InvokeAsync_WhenExceptionThrown_ShouldHandleExceptionAndSetResponse()
    {
        // Arrange
        var expectedException = new BusinessException("A business validation failed");
        _nextMock.Setup(next => next(_httpContext)).ThrowsAsync(expectedException);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _nextMock.Verify(next => next(_httpContext), Times.Once);

        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        _httpContext.Response.ContentType.Should().Be("application/json");

        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(_httpContext.Response.Body);
        var json = await reader.ReadToEndAsync();

        json.Should().Contain("A business validation failed");
    }
}
