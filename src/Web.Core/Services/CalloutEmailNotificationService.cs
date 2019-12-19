using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Core.Services.Settings.Interfaces;
using AMTools.Web.Core.ViewModels;

namespace AMTools.Web.Core.Services
{
    public class CalloutEmailNotificationService : ICalloutEmailNotificationService
    {
        private readonly IQualificationService _qualificationService;

        public CalloutEmailNotificationService(
            IQualificationService qualificationService)
        {
            _qualificationService = qualificationService;
        }


        public void SendEmail(AlertViewModel alert, CalloutNotificationType calloutNotificationType)
        {
            // Als Header/Einleitung die allgemeinen Infos zur Alarmierung
            // Darunter eine Übersicht mit den zurückgemeldeten Qualifizierungen und deren Anzahlen
            // Darunter eine Tabelle mit den Rückmeldungen (durchnummeriert, nach Timestamp absteigend sortiert)
            string headline = GetHeadline(alert, calloutNotificationType);

            string responseQualifications = GetQualificationSummariesFromUserResponses(alert.UserResponses);
        }

        private string GetHeadline(AlertViewModel alert, CalloutNotificationType calloutNotificationType)
        {
            string suffix = null;
            switch (calloutNotificationType)
            {
                case CalloutNotificationType.NewAlert:
                    suffix = "Neue Alarmierung";
                    break;
                case CalloutNotificationType.NewUserResponse:
                    suffix = "Neue Rückmeldung";
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrWhiteSpace(suffix))
            {
                return alert.Number + " - " + suffix;
            }

            return alert.Number.ToString();
        }

        private string GetQualificationSummariesFromUserResponses(List<UserResponseViewModel> userResponseViewModels)
        {
            if (userResponseViewModels == null || userResponseViewModels.Count == 0)
            {
                return "<h5><em>- Noch keine Rückmeldungen erhalten -</em></h5>";
            }

            var resultValues = new Dictionary<QualificationViewModel, int>();

            // TODO: Test
            List<QualificationViewModel> allQualifications = userResponseViewModels.Select(x => x.Subscriber).SelectMany(x => x.Qualifications).ToList();
            if (allQualifications?.Count > 0)
            {
                resultValues = allQualifications.GroupBy(info => info)
                                        .Select(group => new KeyValuePair<QualificationViewModel, int>(group.Key, group.Count()))
                                        .ToDictionary(x => x.Key, x => x.Value);
            }

            List<QualificationViewModel> allQualificationsFromDb = _qualificationService.GetAll();
            if (allQualificationsFromDb != null)
            {
                foreach (QualificationViewModel qualificationFromDB in allQualificationsFromDb)
                {
                    if (!resultValues.ContainsKey(qualificationFromDB))
                    {
                        resultValues.Add(qualificationFromDB, 0);
                    }
                }
            }

            // TODO: continue...

            return null;
        }
    }
}
