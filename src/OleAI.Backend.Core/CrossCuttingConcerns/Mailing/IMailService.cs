namespace OleAI.Backend.Core.CrossCuttingConcerns.Mailing
{
    public interface IMailService
    {
        Task sendAsync(string to, string subject, string htmlBody);
    }
}
