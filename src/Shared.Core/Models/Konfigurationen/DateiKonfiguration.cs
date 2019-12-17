using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models.Konfigurationen.Interfaces;

namespace AMTools.Shared.Core.Models.Konfigurationen
{
    public class DateiKonfiguration : IDateiKonfiguration
    {
        public string SettingsDatei { get; set; }
        public string SubscriberDatei { get; set; }
        public string AvailabilityDatei { get; set; }
        public string CalloutDatei { get; set; }

        public string Datenbankordner { get; set; }
        public string Datenbankpfad
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Datenbankordner))
                {
                    return null;
                }

                return Path.Combine(Datenbankordner, "AmTools2.db");
            }
        }
    }
}
