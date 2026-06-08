using System;
using System.Collections.Generic;

namespace OleAI.Backend.Application.Features.Users.Dtos.Responses
{
    public class AdminUserListItemDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
