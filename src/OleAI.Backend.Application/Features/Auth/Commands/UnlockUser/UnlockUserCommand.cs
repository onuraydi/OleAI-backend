using MediatR;
using OleAI.Backend.Application.Features.Auth.Dtos.Requests;
using OleAI.Backend.Application.Features.Auth.Dtos.Responses;

namespace OleAI.Backend.Application.Features.Auth.Commands.UnlockUser
{
    public class UnlockUserCommand : IRequest<UnlockedUserResponse>
    {
        public UnlockUserRequest Request { get; set; } = null!;
    }
}
