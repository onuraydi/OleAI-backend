using System.Net;
using System.Security.Claims;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OleAI.Backend.Application.Features.Auth.Commands.Login;
using OleAI.Backend.Application.Features.Auth.Commands.RefreshToken;
using OleAI.Backend.Application.Features.Auth.Dtos.Requests;
using OleAI.Backend.Application.Features.Auth.Dtos.Responses;
using OleAI.Backend.Core.Security.Jwt;
using OleAI.Backend.Presentation.Controllers;
using Xunit;

namespace OleAI.Backend.Presentation.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new AuthController();

        // BaseController uses IMediator via HttpContext.RequestServices
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(x => x.GetService(typeof(IMediator))).Returns(_mockMediator.Object);

        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider.Object
        };

        httpContext.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task Login_ShouldReturnOk_AndSetCookie()
    {
        // Arrange
        var command = new LoginCommand
        {
            Request = new LoginRequest { Email = "test@example.com", Password = "123" }
        };

        var response = new LoggedResponse
        {
            AccessToken = new AccessToken { Token = "access", Expriration = DateTime.UtcNow.AddMinutes(15) },
            RefreshToken = new RefreshToken { Token = "refresh", Expiration = DateTime.UtcNow.AddDays(7) }
        };

        _mockMediator.Setup(m => m.Send(It.IsAny<LoginCommand>(), default))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Login(command);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().Be(response);

        var setCookieHeader = _controller.HttpContext.Response.Headers["Set-Cookie"].ToString();
        setCookieHeader.Should().Contain("refreshToken=refresh");
        setCookieHeader.ToLower().Should().Contain("httponly");
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnUnauthorized_WhenCookieIsEmpty()
    {
        // Arrange - no cookie set in HttpContext

        // Act
        var result = await _controller.RefreshToken();

        // Assert
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnOk_WhenCookieIsPresent()
    {
        // Arrange
        _controller.HttpContext.Request.Headers.Append("Cookie", "refreshToken=valid-refresh-token");

        var response = new RefreshTokenResponse
        {
            AccessToken = new AccessToken { Token = "new-access", Expriration = DateTime.UtcNow.AddMinutes(15) },
            RefreshToken = new RefreshToken { Token = "new-refresh", Expiration = DateTime.UtcNow.AddDays(7) }
        };

        _mockMediator.Setup(m => m.Send(It.IsAny<RefreshTokenCommand>(), default))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.RefreshToken();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().Be(response.AccessToken);

        var setCookieHeader = _controller.HttpContext.Response.Headers["Set-Cookie"].ToString();
        setCookieHeader.Should().Contain("refreshToken=new-refresh");
    }

    [Fact]
    public void Logout_ShouldDeleteCookie_AndReturnOk()
    {
        // Arrange
        _controller.HttpContext.Request.Headers.Append("Cookie", "refreshToken=refresh-token");

        // Act
        var result = _controller.Logout();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();

        var setCookieHeader = _controller.HttpContext.Response.Headers["Set-Cookie"].ToString();
        setCookieHeader.Should().Contain("refreshToken=;"); // Cookie deletion 
    }
}
