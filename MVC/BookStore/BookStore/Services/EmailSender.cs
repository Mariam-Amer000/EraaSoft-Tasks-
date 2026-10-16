using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace BookStore.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var host = _configuration["Email:Host"] ?? "smtp.gmail.com";
            var port = _configuration.GetValue<int?>("Email:Port") ?? 587;
            var username = _configuration["Email:Username"];
            var password = _configuration["Email:Password"];
            var from = _configuration["Email:From"] ?? username;

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(from))
            {
                throw new InvalidOperationException(
                    "Email settings are missing. Configure Email:Username, Email:Password, and Email:From.");
            }

            var client = new SmtpClient(host, port)
            {
                EnableSsl = _configuration.GetValue("Email:EnableSsl", true),
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(username, password)
            };

            return client.SendMailAsync(
                new MailMessage(from, email, subject, htmlMessage)
                {
                    IsBodyHtml = true
                });
        }
    }
}