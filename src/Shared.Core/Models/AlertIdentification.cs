using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMTools.Shared.Core.Models
{
    public class AlertIdentification
    {
        public int Number { get; set; }
        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            return $"Number: {Number}, Timestamp: {Timestamp.ToString(CultureInfo.CurrentCulture)}";
        }
    }
}
