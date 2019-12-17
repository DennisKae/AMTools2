using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;

namespace AMTools.Web.Core.ViewModels
{
    public class AlertViewModel
    {
        /// <summary>Datenbank-Id</summary>
        public int? Id { get; set; }

        public int Number { get; set; }

        public int Status { get; set; }

        public DateTime Timestamp { get; set; }

        public DateTime AlertTimestamp { get; set; }

        public string Text { get; set; }

        public string AlertedSubscribers { get; set; }

        public string Xml { get; set; }

        public bool Enabled { get; set; }

        public DateTime? TimestampOfDeactivation { get; set; }

        public List<UserResponseViewModel> UserResponses { get; set; }
    }
}
