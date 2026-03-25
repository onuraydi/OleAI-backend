namespace OleAI.Backend.Application.Features.Auth.Dtos.Responses
{
    public class RegisteredResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
