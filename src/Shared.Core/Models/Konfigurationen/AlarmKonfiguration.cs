using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMTools.Shared.Core.Models.Konfigurationen
{
    public class AlarmKonfiguration
    {
        [Required]
        public int? SperrfristInMinuten { get; set; }

        /// <summary>Virtueller Desktop, auf dem die Rückmeldungen im Alarmfall angezeigt werden. Nicht 0-basiert!</summary>
        [Required]
        public int? AlarmierungsDesktop { get; set; }

        /// <summary>Virtueller Desktop, auf dem das Standby-Bild angezeigt wird.</summary>
        [Required]
        public int? StandbyDesktop { get; set; }

        /// <summary>Liste mit Email-Adressen, die bei Alarmierungen benachrichtigt werden sollen.</summary>
        public List<string> AlarmierungsEmailEmpfaenger { get; set; }

    }
}
