using BAL.Interface;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using MailKit.Net.Smtp;

using System.Text;
using System.Threading.Tasks;

namespace BAL.Repositry
{
    public class EmailRepo:IEmailService
    {
        private readonly IConfiguration _configuration; 
        public EmailRepo(IConfiguration configuration) { 
            _configuration = configuration;
        }
        public bool IsSendEmail(string toEmail, string subject, string body)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var message = new MimeMessage();
                var from = new MailboxAddress(emailSettings["SenderName"], emailSettings["SenderEmail"]);
                var to = new MailboxAddress("", toEmail);
                message.From.Add(from);
                message.To.Add(to);
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = body;

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect(emailSettings["SmtpServer"], int.Parse(emailSettings["SmtpPort"]));
                    client.Authenticate(emailSettings["SmtpUsername"], emailSettings["SmtpPassword"]);
                    client.Send(message);
                    client.Disconnect(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                // Handle exceptions here, you can log the exception for debugging purposes
                // and return false to indicate that the email sending failed
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }

    }
}
