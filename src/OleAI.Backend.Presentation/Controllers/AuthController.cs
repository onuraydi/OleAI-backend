using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OleAI.Backend.Application.Features.Auth.Commands.ChangePassword;
using OleAI.Backend.Application.Features.Auth.Commands.ForgotPassword;
using OleAI.Backend.Application.Features.Auth.Commands.Login;
using OleAI.Backend.Application.Features.Auth.Commands.RefreshToken;
using OleAI.Backend.Application.Features.Auth.Commands.Register;
using OleAI.Backend.Application.Features.Auth.Commands.ResendOtp;
using OleAI.Backend.Application.Features.Auth.Commands.ResetPassword;
using OleAI.Backend.Application.Features.Auth.Commands.VerifyEmail;
using OleAI.Backend.Application.Features.Auth.Dtos.Requests;
using OleAI.Backend.Core.Security.Jwt;

namespace OleAI.Backend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            command.IpAddress = GetIpAddress();

            var result = await Mediator.Send(command);

            SetRefreshTokenToCookie(result.RefreshToken);

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new { message = "Oturum süreniz dolmuş. Lütfen tekrar giriş yapın." });

            var command = new RefreshTokenCommand
            {
                Request = new RefreshTokenRequest
                {
                    Token = refreshToken,
                    IpAddress = GetIpAddress()
                }
            };

            var result = await Mediator.Send(command);

            SetRefreshTokenToCookie(result.RefreshToken);

            return Ok(result.AccessToken);
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            command.Request.UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("refreshToken");
            return Ok(new { message = "Başarıyla çıkış yapıldı." });
        }

        // Yardımcı metotlar (GetIpAddress, SetRefreshTokenToCookie)
        private string GetIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"]!;

            return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "0.0.0.0";
        }

        private void SetRefreshTokenToCookie(RefreshToken refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.Expiration,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
        }
    }
}
