namespace OleAI.Backend.Core.Security.Jwt
{
    public class RefreshToken
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
    }
}
