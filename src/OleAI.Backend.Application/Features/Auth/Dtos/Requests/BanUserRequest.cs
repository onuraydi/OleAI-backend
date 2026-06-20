using System;

namespace OleAI.Backend.Application.Features.Auth.Dtos.Requests
{
    public class BanUserRequest
    {
        public Guid UserId { get; set; }
    }
}
