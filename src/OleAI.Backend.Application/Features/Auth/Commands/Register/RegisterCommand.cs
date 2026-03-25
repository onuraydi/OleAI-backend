using MediatR;
using OleAI.Backend.Application.Features.Auth.Dtos.Requests;
using OleAI.Backend.Application.Features.Auth.Dtos.Responses;

namespace OleAI.Backend.Application.Features.Auth.Commands.Register
{
    public class RegisterCommand : IRequest<RegisteredResponse>
    {
        public RegisterRequest Request { get; set; } = default!;
    }
}
