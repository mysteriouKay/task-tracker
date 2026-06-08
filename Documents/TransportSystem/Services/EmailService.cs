using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace TransportSystem.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string toName, string subject, string body)
        {
            try
            {
                Console.WriteLine($"[EMAIL] Sending to {toEmail}");
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Transport System", _config["Email:Username"]));
                message.To.Add(new MailboxAddress(toName, toEmail));
                message.Subject = subject;
                message.Body = new TextPart("html") { Text = body };

                using var client = new SmtpClient();
                await client.ConnectAsync(_config["Email:Host"], int.Parse(_config["Email:Port"]!), SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_config["Email:Username"], _config["Email:Password"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                Console.WriteLine($"[EMAIL] Sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL] FAILED: {ex.Message}");
            }
        }
    }
}