namespace OleAI.Backend.Application.Features.Auth.Dtos.Requests
{
    public class RefreshTokenRequest
    {
        public string Token { get; set; } = string.Empty; // Eski Refresh Token stringi
        public string IpAddress { get; set; } = string.Empty;
    }
}
