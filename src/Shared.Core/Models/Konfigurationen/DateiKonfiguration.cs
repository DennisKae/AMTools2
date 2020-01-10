using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMTools.Shared.Core.Models.Konfigurationen
{
    public class DateiKonfiguration
    {
        [Required]
        public string SettingsDatei { get; set; }

        [Required]
        public string SubscriberDatei { get; set; }

        [Required]
        public string AvailabilityDatei { get; set; }

        [Required]
        [IsExistingFile]
        public string CalloutDatei { get; set; }

        [Required]
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
