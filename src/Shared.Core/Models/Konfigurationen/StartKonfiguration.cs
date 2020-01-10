using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMTools.Shared.Core.Models.Konfigurationen
{
    public class StartKonfiguration
    {
        [Required]
        public int? WartezeitInMillisekunden { get; set; }

        [Required]
        public int? AnzahlValidierungsversuche { get; set; }

        [Required]
        public List<Desktopinhalt> Desktopinhalte { get; set; }

        [Required]
        public int? DesktopNachDemStart { get; set; }
    }
}
