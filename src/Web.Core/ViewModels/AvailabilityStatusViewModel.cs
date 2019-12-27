using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Web.Core.ViewModels.Settings;

namespace AMTools.Web.Core.ViewModels
{
    public class AvailabilityStatusViewModel
    {
        public string Issi { get; set; }

        public int Value { get; set; }

        public DateTime Timestamp { get; set; }

        public AvailabilityStatusSettingViewModel Setting { get; set; }
    }
}
