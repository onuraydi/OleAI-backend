using OleAI.Backend.Core.Security.Jwt;

namespace OleAI.Backend.Application.Features.Auth.Dtos.Responses
{
    public class LoggedResponse
    {
        public AccessToken AccessToken { get; set; } = null!;
        public RefreshToken RefreshToken { get; set; } = null!;
    }
}
