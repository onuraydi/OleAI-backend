using System;

namespace OleAI.Backend.Application.Features.Auth.Dtos.Requests
{
    public class UnlockUserRequest
    {
        public Guid UserId { get; set; }
    }
}
