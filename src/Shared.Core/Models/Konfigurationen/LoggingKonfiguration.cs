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
        [Required]
        public AppLogSeverity? EmailSchwelle { get; set; }

        [Required]
        public List<string> EmailEmpfaenger { get; set; }
    }
}
