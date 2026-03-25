namespace OleAI.Backend.Application.Features.Auth.Dtos.Requests
{
    public class VerifyEmailRequest
    {
        public string Email { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
    }
}
