using System.Net;
using System.Security.Claims;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OleAI.Backend.Application.Features.Auth.Commands.ChangePassword;
using OleAI.Backend.Application.Features.Auth.Commands.ForgotPassword;
using OleAI.Backend.Application.Features.Auth.Commands.Login;
using OleAI.Backend.Application.Features.Auth.Commands.Logout;
using OleAI.Backend.Application.Features.Auth.Commands.RefreshToken;
using OleAI.Backend.Application.Features.Auth.Commands.Register;
using OleAI.Backend.Application.Features.Auth.Commands.ResendOtp;
using OleAI.Backend.Application.Features.Auth.Commands.ResetPassword;
using OleAI.Backend.Application.Features.Auth.Commands.VerifyEmail;
using OleAI.Backend.Application.Features.Auth.Constants;
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

    // ──────────────────────────────────────────────
    // Login
    // ──────────────────────────────────────────────

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
    public async Task Login_WithXForwardedForHeader_ShouldUseForwardedIp()
    {
        // Arrange
        _controller.HttpContext.Request.Headers.Append("X-Forwarded-For", "203.0.113.42");

        var response = new LoggedResponse
        {
            AccessToken = new AccessToken { Token = "access", Expriration = DateTime.UtcNow.AddMinutes(15) },
            RefreshToken = new RefreshToken { Token = "refresh", Expiration = DateTime.UtcNow.AddDays(7) }
        };

        LoginCommand? capturedCommand = null;
        _mockMediator.Setup(m => m.Send(It.IsAny<LoginCommand>(), default))
            .Callback<IRequest<LoggedResponse>, CancellationToken>((cmd, _) => capturedCommand = (LoginCommand)cmd)
            .ReturnsAsync(response);

        // Act
        await _controller.Login(new LoginCommand { Request = new LoginRequest { Email = "t@t.com", Password = "p" } });

        // Assert – IpAddress must be the forwarded header value, not RemoteIpAddress
        capturedCommand!.IpAddress.Should().Be("203.0.113.42");
    }

    // ──────────────────────────────────────────────
    // RefreshToken
    // ──────────────────────────────────────────────

    [Fact]
    public async Task RefreshToken_ShouldReturnUnauthorized_WhenCookieIsEmpty()
    {
        // Arrange – no cookie set in HttpContext

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

    // ──────────────────────────────────────────────
    // Logout
    // ──────────────────────────────────────────────

    [Fact]
    public async Task Logout_ShouldDeleteCookie_AndReturnOk()
    {
        // Arrange
        _controller.HttpContext.Request.Headers.Append("Cookie", "refreshToken=refresh-token");

        _mockMediator.Setup(m => m.Send(It.IsAny<LogoutCommand>(), default))
            .ReturnsAsync(new LoggedOutResponse { Message = "Logged out successfully" });

        // Act
        var result = await _controller.Logout();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();

        var setCookieHeader = _controller.HttpContext.Response.Headers["Set-Cookie"].ToString();
        setCookieHeader.Should().Contain("refreshToken=;"); // Cookie deletion
    }

    // ──────────────────────────────────────────────
    // Register
    // ──────────────────────────────────────────────

    [Fact]
    public async Task Register_ShouldReturnOk()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Request = new RegisterRequest { Email = "new@example.com", Password = "pass", FirstName = "Ad", LastName = "Soyad" }
        };

        var response = new RegisteredResponse { Id = Guid.NewGuid(), Email = "new@example.com", Message = AuthMessages.SuccessfulRegistration };

        _mockMediator.Setup(m => m.Send(It.IsAny<RegisterCommand>(), default))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Register(command);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().Be(response);
    }

    // ──────────────────────────────────────────────
    // VerifyEmail
    // ──────────────────────────────────────────────

    [Fact]
    public async Task VerifyEmail_ShouldReturnOk()
    {
        // Arrange
        var command = new VerifyEmailCommand
        {
            Request = new VerifyEmailRequest { Email = "test@example.com", OtpCode = "123456" }
        };

        var response = new verifiedEmailResponse { Email = "test@example.com", Message = AuthMessages.UserVerifiedMessage };

        _mockMediator.Setup(m => m.Send(It.IsAny<VerifyEmailCommand>(), default))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.VerifyEmail(command);

        // Assert
        (result as OkObjectResult).Should().NotBeNull();
        ((OkObjectResult)result).Value.Should().Be(response);
    }

    // ──────────────────────────────────────────────
    // ForgotPassword
    // ──────────────────────────────────────────────

    [Fact]
    public async Task ForgotPassword_ShouldReturnOk()
    {
        // Arrange
        var command = new ForgotPasswordCommand
        {
            Request = new ForgotPasswordRequest { Email = "test@example.com" }
        };

        var response = new ForgotPasswordResponse { Message = AuthMessages.RecoveryEmailSentMessage };

        _mockMediator.Setup(m => m.Send(It.IsAny<ForgotPasswordCommand>(), default))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.ForgotPassword(command);

        // Assert
        (result as OkObjectResult).Should().NotBeNull();
    }

    // ──────────────────────────────────────────────
    // ResetPassword
    // ──────────────────────────────────────────────

    [Fact]
    public async Task ResetPassword_ShouldReturnOk()
    {
        // Arrange
        var command = new ResetPasswordCommand
        {
            Request = new ResetPasswordRequest { Email = "test@example.com", OtpCode = "123456", NewPassword = "NewPass!" }
        };

        var response = new ResetPasswordResponse { Message = AuthMessages.AccountRecoverySuccess };

        _mockMediator.Setup(m => m.Send(It.IsAny<ResetPasswordCommand>(), default))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.ResetPassword(command);

        // Assert
        (result as OkObjectResult).Should().NotBeNull();
    }

    // ──────────────────────────────────────────────
    // ResendOtp
    // ──────────────────────────────────────────────

    [Fact]
    public async Task ResendOtp_ShouldReturnOk()
    {
        // Arrange
        var command = new ResendOtpCommand
        {
            Request = new ResendOtpCodeRequest { Email = "test@example.com" }
        };

        var response = new ResendedOtpCodeResponse { Message = AuthMessages.ResendOtpCodeSuccessfully };

        _mockMediator.Setup(m => m.Send(It.IsAny<ResendOtpCommand>(), default))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.ResendOtp(command);

        // Assert
        (result as OkObjectResult).Should().NotBeNull();
    }

    // ──────────────────────────────────────────────
    // ChangePassword  (requires authenticated user)
    // ──────────────────────────────────────────────

    [Fact]
    public async Task ChangePassword_WithAuthenticatedUser_ShouldReturnOk()
    {
        // Arrange – inject authenticated user id into HttpContext
        var userId = Guid.NewGuid();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        }));
        _controller.ControllerContext.HttpContext.User = claimsPrincipal;

        var request = new ChangePasswordRequest { OldPassword = "old", NewPassword = "newPass1!" };
        var response = new ChangePasswordResponse { Message = AuthMessages.SecurityKeyUpdateSuccess };

        _mockMediator.Setup(m => m.Send(It.IsAny<ChangePasswordCommand>(), default))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.ChangePassword(request);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().Be(response);

        // Ensure UserId from claim was forwarded correctly
        _mockMediator.Verify(m => m.Send(
            It.Is<ChangePasswordCommand>(cmd => cmd.UserId == userId),
            default), Times.Once);
    }
}
