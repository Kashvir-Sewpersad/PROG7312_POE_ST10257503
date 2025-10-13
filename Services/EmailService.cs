// Services/EmailService.cs
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace Programming_7312_Part_1.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, string? toName = null)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");

                var smtpServer = emailSettings["SmtpServer"];
                var smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587");
                var senderEmail = emailSettings["SenderEmail"];
                var senderName = emailSettings["SenderName"];
                var username = emailSettings["Username"];
                var password = emailSettings["Password"];
                var enableSsl = bool.Parse(emailSettings["EnableSsl"] ?? "true");

                Console.WriteLine($"Attempting to send email to {toEmail} via {smtpServer}:{smtpPort}");

                using var client = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = enableSsl,
                    Timeout = 30000 // 30 seconds timeout
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail!, senderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(new MailAddress(toEmail, toName));

                await client.SendMailAsync(mailMessage);
                Console.WriteLine($"Email sent successfully to {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (in a real app, you'd use a proper logging framework)
                Console.WriteLine($"Email sending failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task<bool> SendContactConfirmationAsync(string recipientEmail, string recipientName, string subject, string message)
        {
            var emailBody = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #357EC7; color: white; padding: 20px; text-align: center; }}
                        .content {{ padding: 20px; background-color: #f9f9f9; }}
                        .footer {{ background-color: #333; color: white; padding: 10px; text-align: center; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Municipal Services</h1>
                            <p>Contact Confirmation</p>
                        </div>
                        <div class='content'>
                            <h2>Dear {recipientName},</h2>
                            <p>Thank you for contacting Municipal Services. We have received your message and will respond to you as soon as possible.</p>

                            <h3>Your Message Details:</h3>
                            <p><strong>Subject:</strong> {subject}</p>
                            <p><strong>Message:</strong></p>
                            <p>{message.Replace("\n", "<br>")}</p>

                            <p>If you have any additional information or need to follow up, please don't hesitate to contact us again.</p>

                            <p>Best regards,<br>
                            Municipal Services Team</p>
                        </div>
                        <div class='footer'>
                            <p>This is an automated confirmation email. Please do not reply to this message.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(recipientEmail, "Contact Confirmation - Municipal Services", emailBody, recipientName);
        }
    }
}