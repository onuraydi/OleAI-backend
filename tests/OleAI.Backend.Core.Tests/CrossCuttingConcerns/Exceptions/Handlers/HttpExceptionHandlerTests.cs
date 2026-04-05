using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Handlers;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.HttpProblemDetails;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;

namespace OleAI.Backend.Core.Tests.CrossCuttingConcerns.Exceptions.Handlers;

public class HttpExceptionHandlerTests
{
    private readonly HttpExceptionHandler _handler;
    private readonly DefaultHttpContext _httpContext;

    public HttpExceptionHandlerTests()
    {
        _handler = new HttpExceptionHandler();
        _httpContext = new DefaultHttpContext();
        _httpContext.Response.Body = new MemoryStream();
        _handler.Response = _httpContext.Response;
    }

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private async Task<T> GetResponseDetail<T>()
    {
        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var result = await JsonSerializer.DeserializeAsync<T>(_httpContext.Response.Body, _jsonOptions);
        return result!;
    }

    [Fact]
    public async Task HandleException_GivenAuthenticationException_ShouldReturn403Forbidden()
    {
        // Arrange
        var exception = new AuthenticationException("Authentication error occurred");

        // Act
        await _handler.HandleException(exception);

        // Assert
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        _httpContext.Response.ContentType.Should().Be("application/json");

        var detail = await GetResponseDetail<AuthenticationProblemDetails>();
        detail.Should().NotBeNull();
        detail.Detail.Should().Be("Authentication error occurred");
        detail.Type.Should().Be("https://hatasayfasi");
    }

    [Fact]
    public async Task HandleException_GivenAuthorizationException_ShouldReturn401Unauthorized()
    {
        // Arrange
        var exception = new AuthorizationException("Authorization error occurred");

        // Act
        await _handler.HandleException(exception);

        // Assert
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        _httpContext.Response.ContentType.Should().Be("application/json");

        var detail = await GetResponseDetail<AuthorizationProblemDetails>();
        detail.Should().NotBeNull();
        detail.Detail.Should().Be("Authorization error occurred");
    }

    [Fact]
    public async Task HandleException_GivenBusinessException_ShouldReturn400BadRequest()
    {
        // Arrange
        var exception = new BusinessException("Business logic error");

        // Act
        await _handler.HandleException(exception);

        // Assert
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        var detail = await GetResponseDetail<BusinessProblemDetails>();
        detail.Should().NotBeNull();
        detail.Detail.Should().Be("Business logic error");
    }

    [Fact]
    public async Task HandleException_GivenInternalException_ShouldReturn500InternalServerError()
    {
        // Arrange
        var exception = new InternalException("Internal server error occurred");

        // Act
        await _handler.HandleException(exception);

        // Assert
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        var detail = await GetResponseDetail<InternalServerProblemDetails>();
        detail.Should().NotBeNull();
        detail.Detail.Should().Be("Internal server error occurred");
    }

    [Fact]
    public async Task HandleException_GivenExternalException_ShouldReturn502BadGateway()
    {
        // Arrange
        var exception = new ExternalException("External service failed");

        // Act
        await _handler.HandleException(exception);

        // Assert
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status502BadGateway);

        var detail = await GetResponseDetail<ExternalServerProblemDetails>();
        detail.Should().NotBeNull();
        detail.Detail.Should().Be("External service failed");
    }

    [Fact]
    public async Task HandleException_GivenNotFoundException_ShouldReturn404NotFound()
    {
        // Arrange
        var exception = new NotFoundException("Resource not found");

        // Act
        await _handler.HandleException(exception);

        // Assert
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        var detail = await GetResponseDetail<NotFoundProblemDetails>();
        detail.Should().NotBeNull();
        detail.Detail.Should().Be("Resource not found");
    }

    [Fact]
    public async Task HandleException_GivenValidationException_ShouldReturn400BadRequest()
    {
        // Arrange
        var exception = new ValidationException("Validation failed");

        // Act
        await _handler.HandleException(exception);

        // Assert
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        var detail = await GetResponseDetail<ValidationProblemDetails>();
        detail.Should().NotBeNull();
        detail.Detail.Should().Be("Validation failed");
    }

    [Fact]
    public async Task HandleException_GivenGenericException_ShouldReturn500InternalServerError()
    {
        // Arrange
        var exception = new Exception("A generic unhandled exception");

        // Act
        await _handler.HandleException(exception);

        // Assert
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        var detail = await GetResponseDetail<InternalServerProblemDetails>();
        detail.Should().NotBeNull();
        detail.Detail.Should().Be("Sunucu içi bir hata oluştu.");
    }

    [Fact]
    public void ResponseProperty_WhenNotSet_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var handler = new HttpExceptionHandler();

        // Act
        Action act = () => { var res = handler.Response; };

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("*_response*");
    }
}
