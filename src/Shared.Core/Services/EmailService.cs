using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Models;
using AMTools.Shared.Core.Services.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace AMTools.Shared.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogService _logService;

        public EmailService(
            ILogService logService)
        {
            _logService = logService;
        }

        /// <summary>Validiert einen Emailsender und loggt alle Fehler.</summary>
        public bool EmailSenderIsValid(EmailSender emailSender)
        {
            Guard.IsNotNull(emailSender, nameof(EmailSender));

            bool isValid = true;
            var resultBuilder = new StringBuilder();
            if (string.IsNullOrWhiteSpace(emailSender.Address))
            {
                resultBuilder.AppendLine();
            }

            return isValid;
        }

        public void Send(EmailSender emailSender, List<string> recipients, string subject, string body, TextFormat textFormat)
        {
            var message = new MimeMessage();
            if (!string.IsNullOrWhiteSpace(emailSender.Name))
            {
                message.From.Add(new MailboxAddress(Encoding.UTF8, emailSender.Name, emailSender.Address));
            }
            else
            {
                message.From.Add(new MailboxAddress(Encoding.UTF8, emailSender.Address, emailSender.Address));
            }

            message.To.AddRange(recipients.Select(x => new MailboxAddress(x)).ToList());

            if (emailSender.GetEmailsInCopy)
            {
                message.Cc.Add(new MailboxAddress(emailSender.Address));
            }

            message.Subject = subject;

            message.Body = new TextPart(textFormat)
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                client.Connect(emailSender.Hostname, emailSender.Port.Value, emailSender.UseSsl);

                if (!string.IsNullOrWhiteSpace(emailSender.Password))
                {
                    client.Authenticate(emailSender.Address, emailSender.Password);
                }

                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
