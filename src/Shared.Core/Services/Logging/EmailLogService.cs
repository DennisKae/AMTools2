using System;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Models.Konfigurationen;
using AMTools.Shared.Core.Repositories.Interfaces;
using AMTools.Shared.Core.Services.Interfaces;
using AMTools.Web.Data.Database.Models;
using MimeKit.Text;

namespace AMTools.Shared.Core.Services.Logging
{
    public class EmailLogService : LogServiceBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogService _fallbackLogService;
        private readonly IConfigurationFileRepository _configurationFileRepository;

        public EmailLogService(
            IEmailService emailService,
            ILogService fallbackLogService,
            IConfigurationFileRepository configurationFileRepository,
            string assemblyName,
            string batchCommand) : base(assemblyName, batchCommand)
        {
            _emailService = emailService;
            _fallbackLogService = fallbackLogService;
            _configurationFileRepository = configurationFileRepository;
        }

        public override void Log(AppLogSeverity logSeverity, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            string emailSubject = GetHeadline(logSeverity);
            try
            {
                LoggingKonfiguration loggingKonfiguration = _configurationFileRepository.GetConfigFromJsonFile<LoggingKonfiguration>();
                if (logSeverity < loggingKonfiguration.EmailSchwelle.Value || loggingKonfiguration.EmailEmpfaenger == null || loggingKonfiguration.EmailEmpfaenger.Count == 0)
                {
                    return;
                }
                EmailSenderKonfiguration emailSenderKonfiguration = _configurationFileRepository.GetConfigFromJsonFile<EmailSenderKonfiguration>();

                _emailService.Send(emailSenderKonfiguration, loggingKonfiguration.EmailEmpfaenger, emailSubject, message, TextFormat.Text);
            }
            catch (Exception exception)
            {
                _fallbackLogService.Exception(exception, "Beim Versand dieser Log-Email trat eine Exception auf: "
                    + Environment.NewLine
                    + emailSubject
                    + Environment.NewLine
                    + message);
            }
        }
    }
}
