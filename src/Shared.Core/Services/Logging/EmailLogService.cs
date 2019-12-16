//using System;
//using AMTools.Core.Models;
//using AMTools.Core.Models.Configurations;
//using AMTools.Core.Models.Configurations.Interfaces;
//using AMTools.Core.Services.Interfaces;
//using AMTools.Shared.Core.Services.Logging;
//using AMTools.Web.Data.Database.Models;

//namespace AMTools.Core.Services.Logging
//{
//    public class EmailLogService : LogServiceBase
//    {
//        private readonly IEmailService _emailService;
//        private readonly ILogConfig _logConfig;
//        private readonly EmailConfig _emailConfig;
//        private readonly ILogService _fallbackLogService;

//        public EmailLogService(IEmailService emailService, ILogConfig logConfig, EmailConfig emailConfig, ILogService fallbackLogService, string assemblyName, string batchCommand) : base(assemblyName, batchCommand)
//        {
//            _emailService = emailService;
//            _logConfig = logConfig;
//            _emailConfig = emailConfig;
//            _fallbackLogService = fallbackLogService;
//        }

//        public override void Log(AppLogSeverity logSeverity, string message)
//        {
//            if (string.IsNullOrWhiteSpace(message) || logSeverity < _logConfig.EmailThreshold || _emailConfig.EmailSender == null)
//            {
//                return;
//            }

//            string headline = GetHeadline(logSeverity);
//            try
//            {
//                if (_logConfig?.EmailRecipients.Count >= 1 && _emailConfig.EmailSender != null)
//                {
//                    _emailService.Send(_emailConfig.EmailSender, _logConfig.EmailRecipients, headline, message);
//                }
//            }
//            catch (Exception exception)
//            {
//                _fallbackLogService.Exception(exception, "An exception occured while sending the following log email: "
//                    + Environment.NewLine
//                    + headline
//                    + Environment.NewLine
//                    + message);
//            }
//        }
//    }
//}
