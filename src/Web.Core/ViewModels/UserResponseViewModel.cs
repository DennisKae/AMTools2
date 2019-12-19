using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMTools.Web.Core.ViewModels
{
    public class UserResponseViewModel
    {
        public string Issi { get; set; }

        public SubscriberViewModel Subscriber { get; set; }

        public bool Accept { get; set; }

        public string Color { get; set; }

        public string Response { get; set; }

        public DateTime Timestamp { get; set; }

    }
}
