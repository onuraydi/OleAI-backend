using MediatR;
using OleAI.Backend.Application.Features.Auth.Dtos.Requests;
using OleAI.Backend.Application.Features.Auth.Dtos.Responses;

namespace OleAI.Backend.Application.Features.Auth.Commands.BanUser
{
    public class BanUserCommand : IRequest<BanUserResponse>
    {
        public BanUserRequest Request { get; set; } = null!;
    }
}
