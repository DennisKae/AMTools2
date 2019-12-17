using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models.Konfigurationen.Interfaces;

namespace AMTools.Shared.Core.Models.Konfigurationen
{
    public class AlarmKonfiguration : IAlarmKonfiguration
    {
        public int SperrfristInMinuten { get; set; }

        /// <summary>Virtueller Desktop, auf dem die Rückmeldungen im Alarmfall angezeigt werden. Nicht 0-basiert!</summary>
        public int AlarmierungsDesktop { get; set; }

        /// <summary>Virtueller Desktop, auf dem das Standby-Bild angezeigt wird.</summary>
        public int StandbyDesktop { get; set; }
    }
}
