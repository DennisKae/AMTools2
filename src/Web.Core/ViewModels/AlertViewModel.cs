using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Web.Core.ViewModels.Settings;

namespace AMTools.Web.Core.ViewModels
{
    public class AlertViewModel
    {
        /// <summary>Datenbank-Id</summary>
        public int? Id { get; set; }

        public int Number { get; set; }

        public int Status { get; set; }

        public DateTime Timestamp { get; set; }

        public DateTime AlertTimestamp { get; set; }

        public string Text { get; set; }

        public string AlertedSubscribers { get; set; }

        public string Xml { get; set; }

        public bool Enabled { get; set; }

        public DateTime? TimestampOfDeactivation { get; set; }

        public List<UserResponseViewModel> UserResponses { get; set; }

        public string SchweregradText { get; set; }

        public SeverityLevelSettingViewModel Schweregrad { get; set; }

        /// <summary>z.B. "Jonas Färber" oder "Musterhausen"</summary>
        public string TargetText { get; set; }

        /// <summary>Wird nur dann gefüllt, wenn keine SubGroups vorhanden sind.</summary>
        public SubscriberViewModel TargetSubscriber { get; set; }

        public List<SubGroupSettingViewModel> SubGroups { get; set; }

        public string Alarmierungstext { get; set; }
    }
}
