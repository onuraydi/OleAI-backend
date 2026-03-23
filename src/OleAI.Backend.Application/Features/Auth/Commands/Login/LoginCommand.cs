using MediatR;
using Microsoft.AspNetCore.Identity.Data;
using OleAI.Backend.Application.Features.Auth.Dtos.Responses;

namespace OleAI.Backend.Application.Features.Auth.Commands.Login
{
    public class LoginCommand : IRequest<LoggedResponse>
    {
        public LoginRequest Request { get; set; } = default!;
        public string IpAddress { get; set; } = string.Empty;
    }
}
