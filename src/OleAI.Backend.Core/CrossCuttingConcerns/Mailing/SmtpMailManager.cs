using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace OleAI.Backend.Core.CrossCuttingConcerns.Mailing
{
    public class SmtpMailManager : IMailService
    {
        private readonly MailSettings _mailSettings;

        public SmtpMailManager(IConfiguration configuration)
        {
            _mailSettings = configuration.GetSection("MailSettings").Get<MailSettings>()
                ?? throw new ArgumentNullException("MailSettings konfigürasyonu bulunamadı.");
        }

        public async Task sendAsync(string to, string subject, string htmlBody)
        {
            var smtpClient = new SmtpClient(_mailSettings.Server)
            {
                Port = _mailSettings.Port,
                Credentials = new NetworkCredential(_mailSettings.UserName, _mailSettings.Password),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_mailSettings.SenderEmail, _mailSettings.SenderFullName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(to);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
