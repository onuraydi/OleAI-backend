using MediatR;
using Microsoft.AspNetCore.Http;

namespace OleAI.Backend.Application.Features.Users.Commands.UpdateUserProfile
{
    public class UpdateUserProfileCommand : IRequest<string>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public bool IsOnboarded { get; set; } = false;
    }
}
