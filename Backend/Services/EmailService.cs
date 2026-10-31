using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Backend.Services
{
    public class EmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        private readonly string _fromEmail;

        public EmailService(string smtpServer, int smtpPort, string smtpUser, string smtpPass, string fromEmail)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _smtpUser = smtpUser;
            _smtpPass = smtpPass;
            _fromEmail = fromEmail;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                Console.WriteLine($"Attempting to send email to: {toEmail}");
                Console.WriteLine($"SMTP Server: {_smtpServer}:{_smtpPort}");
                Console.WriteLine($"From Email: {_fromEmail}");
                
                var message = new MailMessage(_fromEmail, toEmail, subject, body)
                {
                    IsBodyHtml = true
                };
                
                using var client = new SmtpClient(_smtpServer, _smtpPort)
                {
                    Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                    EnableSsl = true
                };
                
                await client.SendMailAsync(message);
                Console.WriteLine($"Email sent successfully to: {toEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
