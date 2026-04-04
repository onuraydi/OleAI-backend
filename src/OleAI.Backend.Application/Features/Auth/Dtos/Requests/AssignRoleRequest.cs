using System;

namespace OleAI.Backend.Application.Features.Auth.Dtos.Requests
{
    public class AssignRoleRequest
    {
        public Guid UserId { get; set; }
        public Guid OperationClaimId { get; set; }
    }
}
