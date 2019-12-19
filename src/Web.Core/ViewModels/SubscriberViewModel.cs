using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMTools.Web.Core.ViewModels
{
    public class SubscriberViewModel
    {
        public string Issi { get; set; }

        public string Name { get; set; }

        public string Qualification { get; set; }

        public List<QualificationViewModel> Qualifications { get; set; }
    }
}
