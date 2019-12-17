using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace AMTools.Shared.Core.Services
{
    public class EmailService
    {
        public void Send(EmailSender emailSender, string recipient, string subject, string body) => Send(emailSender, new List<string> { recipient }, subject, body);

        public void Send(EmailSender emailSender, List<string> recipients, string subject, string body)
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

            message.Body = new TextPart("plain")
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
