using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core;
using AMTools.Shared.Core.Models;
using AMTools.Shared.Core.Models.Konfigurationen;
using AMTools.Shared.Core.Repositories.Interfaces;
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Core.Services.VirtualDesktops.Interfaces;

namespace AMTools.Web.Core.Services
{
    public class StartupService : IStartupService
    {
        private readonly ILogService _logService;
        private readonly IAlertService _alertService;
        private readonly IVirtualDesktopService _virtualDesktopService;
        private readonly IConfigurationFileRepository _configurationFileRepository;

        public StartupService(
            ILogService logService,
            IAlertService alertService,
            IVirtualDesktopService virtualDesktopService,
            IConfigurationFileRepository configurationFileRepository)
        {
            _logService = logService;
            _alertService = alertService;
            _virtualDesktopService = virtualDesktopService;
            _configurationFileRepository = configurationFileRepository;
        }

        public void ValidateStartKonfiguration()
        {
            _logService.Info($"Validierung der ${nameof(StartKonfiguration)} begonnen.");
            if (!_virtualDesktopService.SwitchingIsAllowed())
            {
                _logService.Info($"Kein Desktopwechsel erlaubt => Überprüfung der {nameof(StartKonfiguration)} beendet.");
                return;
            }
            StartOrValidate(false);
        }

        public void ExecuteStartupConfiguration() => StartOrValidate(false);

        private void StartOrValidate(bool isValidation)
        {
            int previousDesktopId = 0;
            if (isValidation)
            {
                previousDesktopId = _virtualDesktopService.GetIndexOfCurrentDesktop();
            }

            var startKonfiguration = _configurationFileRepository.GetConfigFromJsonFile<StartKonfiguration>();

            int processedStartupItems = 0;
            foreach (var desktopinhalt in startKonfiguration.Desktopinhalte)
            {
                if (!DesktopinhaltIsValid(desktopinhalt))
                {
                    continue;
                }
                processedStartupItems++;

                // Prüfung, ob der Desktopinhalt bereits existiert
                if (!string.IsNullOrWhiteSpace(desktopinhalt.Prozessbezeichnung))
                {
                    int? existingDesktopId = _virtualDesktopService.GetDesktopFromWindowTitle(desktopinhalt.Prozessbezeichnung, true);
                    if (existingDesktopId.HasValue)
                    {
                        if (existingDesktopId.Value == desktopinhalt.Desktop - 1)
                        {
                            _logService.Info($"Der folgende {nameof(Desktopinhalt)} befindet sich bereits auf dem konfigurierten Desktop {existingDesktopId + 1}: " +
                                Environment.NewLine + desktopinhalt.Anzeigetext);
                        }
                        else
                        {
                            _logService.Info(
                                $"Der folgende {nameof(Desktopinhalt)} existierte bereits, befand sich allerdings auf dem falschen Desktop ({existingDesktopId + 1}) und wird bei der später folgenden Überprüfung auf den konfigurierten Desktop ({desktopinhalt.Desktop}) verschoben:" +
                                Environment.NewLine +
                                desktopinhalt?.Anzeigetext ?? desktopinhalt?.Befehl);
                        }

                        continue;
                    }
                }

                _virtualDesktopService.Switch(desktopinhalt.Desktop.Value - 1);

                _logService.Info($"Der folgende Befehl wird auf Desktop {desktopinhalt.Desktop.Value} ausgeführt: " + Environment.NewLine + desktopinhalt.Befehl);

                ProcessStartInfo startInfo = new ProcessStartInfo(desktopinhalt.Befehl)
                {
                    WindowStyle = ProcessWindowStyle.Maximized
                };
                Process startedProcess = Process.Start(startInfo);
            }

            // Manche Anwendungen brauchen zum Starten länger, daher muss die Konfiguration abschließend überprüft werden.
            CheckDesktopsAndMoveWindows(startKonfiguration);

            _logService.Info($"Zusammenfassung: {processedStartupItems} StartupItems verarbeitet.");

            if (isValidation)
            {
                if (_virtualDesktopService.GetIndexOfCurrentDesktop() != previousDesktopId)
                {
                    _logService.Info($"Es wird nun abschließend auf den Desktop ({startKonfiguration.DesktopNachDemStart}) gewechselt, der vor der Programmausführung ausgewählt war...");
                    _virtualDesktopService.Switch(startKonfiguration.DesktopNachDemStart.Value - 1);
                }
            }
            else
            {
                _logService.Info($"Es wird nun abschließend auf den konfigurierten Desktop {startKonfiguration.DesktopNachDemStart} gewechselt...");
                _virtualDesktopService.Switch(startKonfiguration.DesktopNachDemStart.Value - 1);
            }

            return;
        }

        /// <summary>
        /// Überprüft den Desktop der konfigurierten Fenster und verschiebt sie ggf.
        /// </summary>
        private void CheckDesktopsAndMoveWindows(StartKonfiguration startKonfiguration)
        {
            Guard.IsNotNull(startKonfiguration, nameof(StartKonfiguration));

            foreach (Desktopinhalt desktopinhalt in startKonfiguration?.Desktopinhalte)
            {
                if (!DesktopinhaltIsValid(desktopinhalt) || string.IsNullOrWhiteSpace(desktopinhalt?.Prozessbezeichnung))
                {
                    continue;
                }

                int attemptCount = 0;

                do
                {
                    attemptCount++;
                    if (attemptCount > startKonfiguration.AnzahlValidierungsversuche)
                    {
                        _logService.Exception(nameof(CheckDesktopsAndMoveWindows) + $": Der folgende konfigurierte {nameof(Desktopinhalt)} konnte nicht gestartet werden: " + Environment.NewLine + desktopinhalt?.Anzeigetext ?? desktopinhalt?.Befehl);
                        break;
                    }

                    int? actualDesktopId = _virtualDesktopService.GetDesktopFromWindowTitle(desktopinhalt.Prozessbezeichnung, true);
                    if (actualDesktopId.HasValue)
                    {
                        if (actualDesktopId.Value != desktopinhalt.Desktop.Value - 1)
                        {
                            _logService.Info(
                                nameof(CheckDesktopsAndMoveWindows) +
                                $": Der folgende {nameof(Desktopinhalt)} befand sich auf dem falschen Desktop ({actualDesktopId}) und wird auf den konfigurierten Desktop {desktopinhalt.Desktop} verschoben:" +
                                Environment.NewLine +
                                desktopinhalt?.Anzeigetext ?? desktopinhalt?.Befehl);

                            _virtualDesktopService.MoveByWindowTitle(desktopinhalt.Prozessbezeichnung, desktopinhalt.Desktop.Value - 1);
                            break;
                        }

                        // Die Desktop-ID entspricht der Konfiguration
                        break;
                    }

                    _logService.Info($"Warteintervall {attemptCount}/{startKonfiguration.AnzahlValidierungsversuche} ({startKonfiguration.WartezeitInMillisekunden}ms) - Der folgende {nameof(Desktopinhalt)} wurde nicht gefunden:" +
                        Environment.NewLine +
                        desktopinhalt?.Anzeigetext ?? desktopinhalt?.Befehl);
                    Thread.Sleep(startKonfiguration.WartezeitInMillisekunden.Value);
                } while (attemptCount < startKonfiguration.AnzahlValidierungsversuche);
            }
        }

        private bool DesktopinhaltIsValid(Desktopinhalt desktopinhalt)
        {
            if (desktopinhalt == null)
            {
                return true;
            }

            if (!desktopinhalt.Desktop.HasValue)
            {
                _logService.Error($"Invalide {nameof(Desktopinhalt)} Konfiguration: Konfiguration ohne {nameof(desktopinhalt.Desktop)}.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(desktopinhalt.Befehl))
            {
                _logService.Error($"Invalide {nameof(Desktopinhalt)} Konfiguration: Konfiguration ohne {nameof(desktopinhalt.Befehl)}.");
                return false;
            }

            return true;
        }
    }
}
