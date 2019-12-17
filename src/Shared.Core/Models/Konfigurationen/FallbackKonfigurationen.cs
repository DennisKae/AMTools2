using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMTools.Shared.Core.Models.Konfigurationen
{
    public static class FallbackKonfigurationen
    {
        public static readonly AlarmKonfiguration AlarmKonfiguration = new AlarmKonfiguration
        {
            AlarmierungsDesktop = 0,
            StandbyDesktop = 1,
            SperrfristInMinuten = 45
        };
    }
}
