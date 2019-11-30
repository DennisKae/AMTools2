using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMTools.Shared.Core.Models
{
    public class UserResponse
    {

        public string Issi { get; set; }

        public bool Accept { get; set; }

        public string Color { get; set; }

        public string Response { get; set; }

        public DateTime Timestamp { get; set; }

    }
}
