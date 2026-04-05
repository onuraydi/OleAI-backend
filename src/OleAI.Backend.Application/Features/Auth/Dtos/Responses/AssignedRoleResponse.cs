using System;

namespace OleAI.Backend.Application.Features.Auth.Dtos.Responses
{
    public class AssignedRoleResponse
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
