using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMTools.Web.Core.ViewModels.OfficialApplication
{
    /// <summary>ViewModel zur Verwendung mit der offiziellen AlarmMonitor Batch-Schnittstelle</summary>
    public class AlarmApplicationViewModel
    {
        public string Gssi { get; set; }

        public string SubGroup { get; set; }

        public string Text { get; set; }

        public string AlarmId { get; set; }

        public string Severity { get; set; }
    }
}
