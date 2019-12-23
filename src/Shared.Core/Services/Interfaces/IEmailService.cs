using System.Collections.Generic;
using AMTools.Shared.Core.Models;
using MimeKit.Text;

namespace AMTools.Shared.Core.Services.Interfaces
{
    public interface IEmailService
    {
        bool EmailSenderIsValid(EmailSender emailSender);
        void Send(EmailSender emailSender, List<string> recipients, string subject, string body, TextFormat textFormat);
    }
}