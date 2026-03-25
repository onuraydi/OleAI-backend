namespace OleAI.Backend.Core.Security.Jwt
{
    public class AccessToken
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expriration { get; set; }
    }
}
