using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Handlers;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.HttpProblemDetails;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using Xunit;

namespace OleAI.Backend.Core.Tests.CrossCuttingConcerns.Exceptions;

public class HttpExceptionHandlerTests
{
    private readonly HttpExceptionHandler _handler;
    private readonly DefaultHttpContext _context;

    public HttpExceptionHandlerTests()
    {
        _handler = new HttpExceptionHandler();
        _context = new DefaultHttpContext();
        _handler.Response = _context.Response;
    }

    [Fact]
    public void Response_Property_Get_ShouldThrow_WhenNotSet()
    {
        var handler = new HttpExceptionHandler();
        Assert.Throws<InvalidOperationException>(() => handler.Response);
    }

    [Fact]
    public async Task HandleException_BusinessException_ShouldReturn400()
    {
        // Arrange
        var exception = new BusinessException("Business error");
        _context.Response.Body = new MemoryStream();

        // Act
        await _handler.HandleException(exception);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, _context.Response.StatusCode);
        var body = await GetResponseBody(_context.Response);
        var problemDetails = JsonSerializer.Deserialize<BusinessProblemDetails>(body);
        Assert.Equal("Business error", problemDetails?.Detail);
    }

    [Fact]
    public async Task HandleException_AuthenticationException_ShouldReturn403()
    {
        // Arrange
        var exception = new AuthenticationException("Auth error");
        _context.Response.Body = new MemoryStream();

        // Act
        await _handler.HandleException(exception);

        // Assert
        Assert.Equal(StatusCodes.Status403Forbidden, _context.Response.StatusCode);
        var body = await GetResponseBody(_context.Response);
        var problemDetails = JsonSerializer.Deserialize<AuthenticationProblemDetails>(body);
        Assert.Equal("Auth error", problemDetails?.Detail);
    }

    [Fact]
    public async Task HandleException_AuthorizationException_ShouldReturn401()
    {
        // Arrange
        var exception = new AuthorizationException("Authz error");
        _context.Response.Body = new MemoryStream();

        // Act
        await _handler.HandleException(exception);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, _context.Response.StatusCode);
        var body = await GetResponseBody(_context.Response);
        var problemDetails = JsonSerializer.Deserialize<AuthorizationProblemDetails>(body);
        Assert.Equal("Authz error", problemDetails?.Detail);
    }

    [Fact]
    public async Task HandleException_NotFoundException_ShouldReturn404()
    {
        // Arrange
        var exception = new NotFoundException("Not found");
        _context.Response.Body = new MemoryStream();

        // Act
        await _handler.HandleException(exception);

        // Assert
        Assert.Equal(StatusCodes.Status404NotFound, _context.Response.StatusCode);
        var body = await GetResponseBody(_context.Response);
        var problemDetails = JsonSerializer.Deserialize<NotFoundProblemDetails>(body);
        Assert.Equal("Not found", problemDetails?.Detail);
    }

    [Fact]
    public async Task HandleException_ValidationException_ShouldReturn400()
    {
        // Arrange
        var exception = new ValidationException("Validation error");
        _context.Response.Body = new MemoryStream();

        // Act
        await _handler.HandleException(exception);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, _context.Response.StatusCode);
        var body = await GetResponseBody(_context.Response);
        var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(body);
        Assert.Equal("Validation error", problemDetails?.Detail);
    }

    [Fact]
    public async Task HandleException_InternalException_ShouldReturn500()
    {
        // Arrange
        var exception = new InternalException("Internal error");
        _context.Response.Body = new MemoryStream();

        // Act
        await _handler.HandleException(exception);

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, _context.Response.StatusCode);
        var body = await GetResponseBody(_context.Response);
        var problemDetails = JsonSerializer.Deserialize<InternalServerProblemDetails>(body);
        Assert.Equal("Internal error", problemDetails?.Detail);
    }

    [Fact]
    public async Task HandleException_ExternalException_ShouldReturn502()
    {
        // Arrange
        var exception = new ExternalException("External error");
        _context.Response.Body = new MemoryStream();

        // Act
        await _handler.HandleException(exception);

        // Assert
        Assert.Equal(StatusCodes.Status502BadGateway, _context.Response.StatusCode);
        var body = await GetResponseBody(_context.Response);
        var problemDetails = JsonSerializer.Deserialize<ExternalServerProblemDetails>(body);
        Assert.Equal("External error", problemDetails?.Detail);
    }

    [Fact]
    public async Task HandleException_GenericException_ShouldReturn500()
    {
        // Arrange
        var exception = new Exception("Generic error");
        _context.Response.Body = new MemoryStream();

        // Act
        await _handler.HandleException(exception);

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, _context.Response.StatusCode);
        var body = await GetResponseBody(_context.Response);
        var problemDetails = JsonSerializer.Deserialize<InternalServerProblemDetails>(body);
        Assert.Equal("Generic error", problemDetails?.Detail);
    }

    [Fact]
    public async Task HandleExceptionAsync_Base_ShouldDispatchCorrectType()
    {
        // Arrange
        var exception = new BusinessException("Business dispatch");
        _context.Response.Body = new MemoryStream();

        // Act
        await _handler.HandleExceptionAsync(exception);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, _context.Response.StatusCode);
    }

    private async Task<string> GetResponseBody(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(response.Body, Encoding.UTF8);
        return await reader.ReadToEndAsync();
    }
}
