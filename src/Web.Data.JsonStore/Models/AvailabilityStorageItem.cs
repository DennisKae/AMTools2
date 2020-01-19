using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMTools.Web.Data.JsonStore.Models
{
    public class AvailabilityStorageItem
    {
        public int SubscriberId { get; set; }

        public string AvailabilityKey { get; set; }

        public DateTime? Timestamp { get; set; }
    }
}
