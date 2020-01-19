using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Web.Core.ViewModels.Settings;

namespace AMTools.Web.Core.ViewModels
{
    public class SubscriberViewModel
    {
        /// <summary>Datenbank-ID</summary>
        public int Id { get; set; }

        public string Issi { get; set; }

        public string Name { get; set; }

        public string Qualification { get; set; }

        public List<QualificationSettingViewModel> Qualifications { get; set; }

        public AvailabilityStatusViewModel AvailabilityStatus { get; set; }
    }
}
