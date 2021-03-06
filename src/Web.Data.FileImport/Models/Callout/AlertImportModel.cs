﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMTools.Web.Data.Files.Models.Callout
{
    internal class AlertImportModel
    {

        public int Number { get; set; }

        public int Status { get; set; }

        public DateTime Timestamp { get; set; }

        public DateTime AlertTimestamp { get; set; }

        public string Text { get; set; }

        public string AlertedSubscribers { get; set; }

        public string Xml { get; set; }

    }
}
