using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMTools.Web.Core.ViewModels
{
    public class GuiVisibilityViewModel
    {
        public bool ShowAvailabilityTimestamp { get; set; }

        public bool SortSubscribersByName { get; set; }

        public bool GroupSubscribersByQualification { get; set; }
    }
}
