using MediatR;
using OleAI.Backend.Application.Features.Auth.Dtos.Responses;

namespace OleAI.Backend.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommand : IRequest<LoggedOutResponse>
    {
        public string RefreshToken { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
    }
}
