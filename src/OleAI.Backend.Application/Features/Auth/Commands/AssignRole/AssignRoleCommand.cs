using MediatR;
using OleAI.Backend.Application.Features.Auth.Dtos.Requests;
using OleAI.Backend.Application.Features.Auth.Dtos.Responses;

namespace OleAI.Backend.Application.Features.Auth.Commands.AssignRole
{
    public class AssignRoleCommand : IRequest<AssignedRoleResponse>
    {
        public AssignRoleRequest Request { get; set; } = null!;
    }
}
