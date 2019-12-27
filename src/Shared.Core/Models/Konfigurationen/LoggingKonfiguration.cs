using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Web.Data.Database.Models;

namespace AMTools.Shared.Core.Models.Konfigurationen
{
    public class LoggingKonfiguration
    {
        public AppLogSeverity? EmailSchwelle { get; set; }

        public List<string> EmailEmpfaenger { get; set; }
    }
}
