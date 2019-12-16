using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models.Konfigurationen.Interfaces;

namespace AMTools.Shared.Core.Models.Konfigurationen
{
    public class FileConfig : IFileConfig
    {
        public string SettingsFile { get; set; }
        public string SubscriberFile { get; set; }
        public string AvailabilityFile { get; set; }
        public string Calloutfile { get; set; }
        public string CalloutfileFilename
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Calloutfile))
                {
                    return null;
                }

                return Path.GetFileName(Calloutfile);
            }
        }
        public string DatabaseDirectory { get; set; }
        public string Databasefile
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DatabaseDirectory))
                {
                    return null;
                }

                return Path.Combine(DatabaseDirectory, "AmTools.db");
            }
        }
    }
}
