using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Web.Data.Database.Models;

namespace AMTools.Shared.Core.Models
{
    public class AppLog
    {

        public DateTime Timestamp { get; set; }

        public AppLogSeverity Severity { get; set; }

        public string Message { get; set; }

        public string ApplicationPart { get; set; }
        public string BatchCommand { get; set; }
    }
}
