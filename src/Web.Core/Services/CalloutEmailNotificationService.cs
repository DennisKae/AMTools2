using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core;
using AMTools.Shared.Core.Models.Konfigurationen;
using AMTools.Shared.Core.Repositories.Interfaces;
using AMTools.Shared.Core.Services.Interfaces;
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Core.Services.Settings.Interfaces;
using AMTools.Web.Core.ViewModels;
using MimeKit.Text;

namespace AMTools.Web.Core.Services
{
    public class CalloutEmailNotificationService : ICalloutEmailNotificationService
    {
        private readonly IQualificationService _qualificationService;
        private readonly IEmailService _emailService;
        private readonly IConfigurationFileRepository _configurationFileRepository;
        private readonly ILogService _logService;
        private readonly string _htmlHead = @"
<head>
    <style>
        body {
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif, 'Apple Color Emoji', 'Segoe UI Emoji', 'Segoe UI Symbol';
        }
        table { border-collapse: collapse }
        td, th {
            border: 1px solid black;
            padding: 0px 5px;
        }
        .text-center { text-align: center; }
        h1, h2, h3, h4, h5, h6 {
            margin-top: 0;
            margin-bottom: .5rem;
        }
        dl, ol, ul, table {
            margin-top: .25rem;
            margin-bottom: 1rem;
        }
    </style>
</head>
";

        public CalloutEmailNotificationService(
            IQualificationService qualificationService,
            IEmailService emailService,
            IConfigurationFileRepository configurationFileRepository,
            ILogService logService)
        {
            _qualificationService = qualificationService;
            _emailService = emailService;
            _configurationFileRepository = configurationFileRepository;
            _logService = logService;
        }


        public void SendEmail(AlertViewModel alert, CalloutNotificationType calloutNotificationType)
        {
            AlarmKonfiguration alarmKonfiguration = _configurationFileRepository.GetConfigFromJsonFile<AlarmKonfiguration>();
            EmailSenderKonfiguration emailSenderKonfiguration = _configurationFileRepository.GetConfigFromJsonFile<EmailSenderKonfiguration>();
            Guard.IsNotNull(alarmKonfiguration, nameof(AlarmKonfiguration));
            Guard.IsNotNull(emailSenderKonfiguration, nameof(EmailSenderKonfiguration));

            if (alarmKonfiguration.AlarmierungsEmailEmpfaenger == null || alarmKonfiguration.AlarmierungsEmailEmpfaenger.Count == 0)
            {
                _logService.Info($"In der {nameof(AlarmKonfiguration)} wurden keine {nameof(alarmKonfiguration.AlarmierungsEmailEmpfaenger)} konfiguriert.");
                return;
            }

            if (!_emailService.EmailSenderIsValid(emailSenderKonfiguration))
            {
                return;
            }

            var emailBodyBuilder = new StringBuilder();
            emailBodyBuilder.AppendLine("<!DOCTYPE html>");
            emailBodyBuilder.AppendLine($"<html>{_htmlHead}<body>");

            // Überschrift
            emailBodyBuilder.AppendLine($"<h2>{GetTextFromNotificationType(calloutNotificationType)}</h2>");

            // Header/Einleitung mit allgemeinen Infos zur Alarmierung
            emailBodyBuilder.AppendLine(GetGeneralAlertInformations(alert));

            if (alert.UserResponses?.Count > 0)
            {
                // Übersicht mit den zurückgemeldeten Qualifizierungen und deren Anzahlen
                emailBodyBuilder.AppendLine(GetQualificationSummariesFromUserResponses(alert.UserResponses));

                // Tabelle mit den jeweiligen Rückmeldungen
                emailBodyBuilder.AppendLine(GetResponseTable(alert.UserResponses));
            }
            else
            {
                emailBodyBuilder.AppendLine("<h3><em>- Noch keine Rückmeldungen erhalten -</em></h3>");
            }
            emailBodyBuilder.AppendLine($"<p><small>Alarmmonitor-Nummer: {alert.Number} <br/>Interne ID: {alert.Id}</small></p>");

            emailBodyBuilder.AppendLine("</body></html>");
            var emailBody = emailBodyBuilder.ToString();
            string emailSubject = GetSubject(alert, calloutNotificationType);

            try
            {
                _emailService.Send(emailSenderKonfiguration, alarmKonfiguration.AlarmierungsEmailEmpfaenger, emailSubject, emailBody, TextFormat.Html);
            }
            catch (Exception exception)
            {
                _logService.Exception(exception, $"Exception beim Emailversand für die Alarmierung mit der ID {alert.Id}.");
            }
        }

        private string GetSubject(AlertViewModel alert, CalloutNotificationType calloutNotificationType)
        {
            string prefix = GetTextFromNotificationType(calloutNotificationType);

            var resultBuilder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(prefix))
            {
                resultBuilder.Append($"#{alert.Id}: " + prefix);
            }

            if (!string.IsNullOrWhiteSpace(alert.Alarmierungstext))
            {
                resultBuilder.Append(" - " + alert.Alarmierungstext);
            }

            return resultBuilder.ToString();
        }

        private string GetTextFromNotificationType(CalloutNotificationType calloutNotificationType)
        {
            switch (calloutNotificationType)
            {
                case CalloutNotificationType.NewAlert:
                    return "Neue Alarmierung";
                case CalloutNotificationType.NewUserResponse:
                    return "Neue Rückmeldung";
                default:
                    return null;
            }
        }

        private string GetGeneralAlertInformations(AlertViewModel alert)
        {
            if (alert == null)
            {
                return null;
            }
            StringBuilder resultBuilder = new StringBuilder();
            resultBuilder.AppendLine("<ul>");
            resultBuilder.AppendLine($"<li><strong>Alarmierungszeit:</strong> {alert.AlertTimestamp}</li>");
            resultBuilder.AppendLine($"<li><strong>Alarmtext:</strong> {alert.Alarmierungstext ?? alert.Text}</li>");
            if (!string.IsNullOrWhiteSpace(alert.Schweregrad?.Bezeichnung) || !string.IsNullOrWhiteSpace(alert.SchweregradText))
            {
                resultBuilder.AppendLine($"<li><strong>Schweregrad:</strong> {alert.Schweregrad?.Bezeichnung ?? alert.SchweregradText}</li>");
            }

            if (alert.SubGroups?.Count > 0)
            {
                resultBuilder.AppendLine($"<li><strong>Alarmierte Gruppe(n):</strong> {string.Join(", ", alert.SubGroups.Select(x => x.Bezeichnung).ToArray())}</li> ");
            }
            else if (alert.TargetSubscriber != null)
            {
                resultBuilder.AppendLine($"<li><strong>Alarmierter Benutzer:</strong> {alert.TargetSubscriber.Name ?? alert.TargetSubscriber.Issi}</li> ");
            }
            else if (!string.IsNullOrWhiteSpace(alert.TargetText))
            {
                resultBuilder.AppendLine($"<li><strong>Ziel der Alarmierung:</strong> {alert.TargetText}</li>");
            }
            resultBuilder.AppendLine("</ul>");
            return resultBuilder.ToString();
        }

        /// <summary>Liefert eine HTML-Tabelle mit einer Zusammenfassung über die Qualifikationen der Subscriber der UserResponses.</summary>
        private string GetQualificationSummariesFromUserResponses(List<UserResponseViewModel> userResponseViewModels)
        {
            if (userResponseViewModels == null || userResponseViewModels.Count == 0)
            {
                return null;
            }

            // Key = Abkürzung der Qualification, Value = Anzahl
            var resultValues = new Dictionary<string, int>();

            List<QualificationViewModel> allQualifications = userResponseViewModels.Select(x => x.Subscriber).SelectMany(x => x.Qualifications).ToList();
            if (allQualifications?.Count > 0)
            {
                resultValues = allQualifications.GroupBy(x => x.Abkuerzung)
                                        .Select(group => new KeyValuePair<string, int>(group.Key, group.Count()))
                                        .ToDictionary(x => x.Key, x => x.Value);
            }

            List<QualificationViewModel> allQualificationsFromDb = _qualificationService.GetAll();

            StringBuilder resultBuilder = new StringBuilder();
            resultBuilder.AppendLine("<h3>Qualifikationen der Rückmeldungen</h3>");
            resultBuilder.AppendLine("<table><thead>");
            resultBuilder.AppendLine("<tr><th>Qualifikation</th><th>Anzahl</th></tr>");
            resultBuilder.AppendLine("</thead><tbody>");
            if (allQualificationsFromDb != null)
            {
                foreach (QualificationViewModel qualificationFromDB in allQualificationsFromDb)
                {
                    int anzahl = resultValues.TryGetValue(qualificationFromDB.Abkuerzung, out int parsedAnzahl) ? parsedAnzahl : 0;
                    resultBuilder.AppendLine($"<tr><td>{qualificationFromDB.Bezeichnung}</td><td class='text-center'>{anzahl}</td></tr>");
                }
            }
            else
            {
                _logService.Error("Keine Qualifications in der Settings-Tabelle gefunden!");
            }
            resultBuilder.AppendLine("</tbody></table>");

            return resultBuilder.ToString();
        }

        /// <summary>Liefert eine HTML-Tabelle mit den UserResponses</summary>
        private string GetResponseTable(List<UserResponseViewModel> userResponses)
        {
            if (userResponses == null || userResponses.Count == 0)
            {
                return null;
            }
            StringBuilder resultBuilder = new StringBuilder();
            resultBuilder.AppendLine($"<h3>Rückmeldungen ({userResponses.Count})</h3>");
            resultBuilder.AppendLine("<table><thead><tr>");
            resultBuilder.AppendLine("<th>Datum</th>");
            resultBuilder.AppendLine("<th>Rückmeldung</th>");
            resultBuilder.AppendLine("<th>Name</th>");
            resultBuilder.AppendLine("<th>Qualifikationen</th>");
            resultBuilder.AppendLine("</tr></thead><tbody>");

            userResponses = userResponses.OrderByDescending(x => x.Timestamp).ToList();
            foreach (UserResponseViewModel userResponse in userResponses)
            {
                resultBuilder.AppendLine("<tr>");

                resultBuilder.AppendLine($"<td>{userResponse.Timestamp}</td>");
                if (userResponse.Accept)
                {
                    resultBuilder.AppendLine($"<td class='text-center' style='background-color:{userResponse.Color}'><strong>Akzeptiert</strong></td>");
                }
                else
                {
                    resultBuilder.AppendLine($"<td class='text-center' style='background-color:{userResponse.Color}'>Abgelehnt</td>");
                }
                resultBuilder.AppendLine($"<td>{userResponse.Subscriber?.Name ?? userResponse.Issi}</td>");
                resultBuilder.AppendLine($"<td>{string.Join(", ", userResponse.Subscriber?.Qualifications?.Select(x => x.Bezeichnung)?.ToArray() ?? new string[] { })}</td>");

                resultBuilder.AppendLine("</tr>");
            }
            resultBuilder.AppendLine("</tbody></table>");

            resultBuilder.AppendLine("<h4>Zusammenfassung:</h4>");
            resultBuilder.AppendLine("<ul>");
            resultBuilder.AppendLine($"<li>{userResponses.Count(x => x.Accept)} akzeptiert</li>");
            resultBuilder.AppendLine($"<li>{userResponses.Count(x => !x.Accept)} abgelehnt</li>");
            resultBuilder.AppendLine("</ul>");

            return resultBuilder.ToString();
        }
    }
}
