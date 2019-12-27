using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMTools.Web.Core.ViewModels.Settings
{
    public class GuiVisibilitySettingViewModel
    {
        public bool ShowAvailabilityTimestamp { get; set; }

        public bool SortSubscribersByName { get; set; }

        public bool GroupSubscribersByQualification { get; set; }
    }
}
