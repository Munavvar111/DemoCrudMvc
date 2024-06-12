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
        public bool IsSendEmail(string ToEmail, string Subject, string Body)
        {
            try
            {
                var EmailSetting = _configuration.GetSection("EmailSettings");
                var Message = new MimeMessage();
                var From = new MailboxAddress(EmailSetting["SenderName"], EmailSetting["SenderEmail"]);
                var To = new MailboxAddress("", ToEmail);
                Message.From.Add(From);
                Message.To.Add(To);
                Message.Subject = Subject;

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = Body;

                Message.Body = bodyBuilder.ToMessageBody();

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect(EmailSetting["SmtpServer"], int.Parse(EmailSetting["SmtpPort"]));
                    client.Authenticate(EmailSetting["SmtpUsername"], EmailSetting["SmtpPassword"]);
                    client.Send(Message);
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
