using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core;
using AMTools.Shared.Core.Models;
using AMTools.Shared.Core.Models.Konfigurationen;
using AMTools.Shared.Core.Repositories.Interfaces;
using AMTools.Shared.Core.Services.Interfaces;
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Core.Services.VirtualDesktops.Interfaces;

namespace AMTools.Web.Core.Services.VirtualDesktops
{
    public class VirtualDesktopService : IVirtualDesktopService
    {
        private readonly ILogService _logService;
        private readonly IVirtualDesktopWrapperService _virtualDesktopService;
        private readonly ITerminalService _terminalService;
        private readonly IAlertService _alertService;
        private readonly IConfigurationFileRepository _configurationFileRepository;

        public VirtualDesktopService(
            ILogService logService,
            IVirtualDesktopWrapperService virtualDesktopWrapperService,
            ITerminalService terminalService,
            IAlertService alertService,
            IConfigurationFileRepository configurationFileRepository
            )
        {
            _logService = logService;
            _virtualDesktopService = virtualDesktopWrapperService;
            _terminalService = terminalService;
            _alertService = alertService;
            _configurationFileRepository = configurationFileRepository;
        }

        public bool SwitchRight()
        {
            _logService.Info("Es wird zum nächsten Desktop auf der rechten Seite umgeschaltet...");

            // Neuen Desktop anlegen, wenn bisher nur 1 Desktop besteht
            int countOfDesktops = GetCountOfVirtualDesktops();
            if (countOfDesktops == 1)
            {
                _logService.Info("Es existiert nur ein Desktop. Ein weiterer Desktop wird erstellt.");
                if (!CreateNewDesktop())
                {
                    return false;
                }
            }

            // Switch to the first Desktop if the current Desktop is the last Desktop
            int indexOfCurrentDesktop = GetIndexOfCurrentDesktop();
            if (indexOfCurrentDesktop + 1 == countOfDesktops)
            {
                return Switch(0);
            }

            TerminalResult terminalResult = _virtualDesktopService.SwitchRight();

            if (terminalResult.ErrorOccured)
            {
                _terminalService.WriteResultToLog(terminalResult, "Beim Umschalten auf den rechten Desktop ist ein Fehler aufgetreten: ");
            }
            return !terminalResult.ErrorOccured;
        }

        public bool SwitchLeft()
        {
            _logService.Info("Es wird zum nächsten Desktop auf der linken Seite umgeschaltet...");

            // Neuen Desktop anlegen, wenn bisher nur 1 Desktop besteht
            int countOfDesktops = GetCountOfVirtualDesktops();
            if (countOfDesktops == 1)
            {
                _logService.Info("Es gibt bisher nur einen Desktop. Ein weiterer Desktop wird erstellt.");
                if (!CreateNewDesktop())
                {
                    return false;
                }
            }

            // Switch to the last Desktop if the current Desktop is the first Desktop
            int indexOfCurrentDesktop = GetIndexOfCurrentDesktop();
            if (indexOfCurrentDesktop == 0)
            {
                return Switch(countOfDesktops - 1);
            }

            TerminalResult terminalResult = _virtualDesktopService.SwitchLeft();

            if (terminalResult.ErrorOccured)
            {
                _terminalService.WriteResultToLog(terminalResult, "Beim Umschalten auf den linken Desktop ist ein Fehler aufgetreten: ");
            }
            return !terminalResult.ErrorOccured;
        }

        public void SwitchWithMultipleAttempts(int targetDesktopNumber) => SwitchWithMultipleAttempts(targetDesktopNumber, null);

        public void SwitchWithMultipleAttempts(int targetDesktopNumber, int maxAttemptCount) => SwitchWithMultipleAttempts(targetDesktopNumber, null, maxAttemptCount);

        public void SwitchWithMultipleAttempts(int targetDesktopNumber, string targetDesktopDescription) => SwitchWithMultipleAttempts(targetDesktopNumber, targetDesktopDescription, 10);

        public void SwitchWithMultipleAttempts(int targetDesktopNumber, string targetDesktopDescription, int maxAttemptCount)
        {
            if (string.IsNullOrWhiteSpace(targetDesktopDescription))
            {
                targetDesktopDescription = "Desktop " + targetDesktopNumber;
            }

            bool hasSwitched = Switch(targetDesktopNumber - 1);
            int attemptCounter = 1;
            while (!hasSwitched && attemptCounter <= maxAttemptCount)
            {
                _logService.Error($"Beim Umschalten auf den {targetDesktopDescription} ({targetDesktopNumber}) trat ein Fehler auf. Die Umschaltung wird zum {attemptCounter + 1}. mal erneut versucht...");
                hasSwitched = Switch(targetDesktopNumber - 1);
                attemptCounter++;
            }

            if (!hasSwitched)
            {
                _logService.Exception($"Es wurde {maxAttemptCount} mal erfolglos versucht auf den {targetDesktopDescription} (Desktop {targetDesktopNumber}) umzuschalten.");
            }
        }

        /// <summary>Wechselt zum ausgewählten Desktop. Liefert true, wenn der Vorgang erfolgreich war.</summary>
        public bool Switch(int targetDesktopIndex)
        {
            _logService.Info($"Es wird zum Desktop mit der ID #{targetDesktopIndex} umgeschaltet...");

            // Create new Desktops if the given Desktop does not exist
            if (!DesktopExists(targetDesktopIndex))
            {
                _logService.Error($"Umschalten auf den Desktop #{targetDesktopIndex} noch nicht möglich: Es existiert kein Desktop mit dieser ID. Neue Desktops werden erstellt...");
                bool newDesktopSuccessfullyCreated;
                do
                {
                    newDesktopSuccessfullyCreated = CreateNewDesktop();
                } while (!DesktopExists(targetDesktopIndex) && newDesktopSuccessfullyCreated);
            }

            TerminalResult terminalResult = _virtualDesktopService.Switch(targetDesktopIndex);

            if (terminalResult.ErrorOccured)
            {
                _terminalService.WriteResultToLog(terminalResult, "Beim Umschalten auf den rechten Desktop ist ein Fehler aufgetreten: ");
            }
            return !terminalResult.ErrorOccured;
        }

        public int GetCountOfVirtualDesktops()
        {
            var terminalResult = _virtualDesktopService.GetCountOfVirtualDesktops();

            return GetIntFromKeyValueString(':', terminalResult.StandardOutput);
        }

        public int GetIndexOfCurrentDesktop()
        {
            var terminalResult = _virtualDesktopService.GetIndexOfCurrentDesktop();
            return GetIntFromKeyValueString(':', terminalResult.StandardOutput);
        }

        public bool DesktopExists(int desktopIndex)
        {
            if (desktopIndex < 0)
            {
                return false;
            }

            int countOfDesktops = GetCountOfVirtualDesktops();
            if (countOfDesktops - 1 < desktopIndex)
            {
                return false;
            }

            return true;
        }

        public bool CreateNewDesktop()
        {
            _logService.Info("Ein neuer Desktop wird erstellt...");
            var terminalResult = _virtualDesktopService.CreateNewDesktop();
            if (terminalResult.ErrorOccured)
            {
                _terminalService.WriteResultToLog(terminalResult, $"Bei der Erstellung eines neuen virt. Desktops ist ein Fehler aufgetreten.");
            }
            else
            {
                _logService.Info("Ein neuer Desktop wurde erstellt.");
            }
            return !terminalResult.ErrorOccured;
        }

        /// <summary>Errors won't be suppressed</summary>
        public int? GetDesktopFromWindowTitle(string windowTitle) => GetDesktopFromWindowTitle(windowTitle, false);

        public int? GetDesktopFromWindowTitle(string windowTitle, bool suppressErrors)
        {
            var terminalResult = _virtualDesktopService.GetDesktopFromWindowTitle(windowTitle);

            if (terminalResult.ErrorOccured)
            {
                if (!suppressErrors)
                {
                    _terminalService.WriteResultToLog(terminalResult, $"Beim Auslesen der Desktop ID des Fensters \"{windowTitle}\" ist ein Fehler aufgetreten.");
                }

                return null;
            }

            string successIndicator = "is on desktop";

            if (terminalResult.StandardOutput?.Contains(successIndicator) != true)
            {
                if (!suppressErrors)
                {
                    _terminalService.WriteResultToLog(terminalResult, $"Es konnte keine Desktop ID des Fensters \"{windowTitle}\" ausgelesen werden.");
                }
                return null;
            }

            var indexAfterSeparator = terminalResult.StandardOutput.IndexOf(successIndicator) + successIndicator.Length;
            string stringResult = terminalResult.StandardOutput.Substring(indexAfterSeparator).TrimEnd('\r', '\n').Trim();
            if (int.TryParse(stringResult, out int result))
            {
                return result;
            }

            if (!suppressErrors)
            {
                _terminalService.WriteResultToLog(terminalResult, $"Es konnte keine Desktop ID des Fensters \"{windowTitle}\" ausgelesen werden.");
            }

            return null;
        }

        public void MoveByWindowTitle(string windowTitle, int targetDesktopIndex)
        {
            _logService.Info($"Das Fenster \"{windowTitle}\" wird auf den virt. Desktop #{targetDesktopIndex} verschoben.");

            if (windowTitle.Contains(" "))
            {
                throw new ArgumentOutOfRangeException(windowTitle, $"Der ${nameof(windowTitle)} darf keine Leerzeichen enthalten.");
            }

            if (!DesktopExists(targetDesktopIndex))
            {
                _logService.Error($"Das Fenster kann noch nicht auf den virt. Desktop #{targetDesktopIndex} verschoben werden: Es existiert noch kein Desktop mit dieser ID. Neue Desktops werden nun erstellt...");
                do
                {
                    CreateNewDesktop();
                } while (!DesktopExists(targetDesktopIndex));
            }


            TerminalResult result = _virtualDesktopService.MoveByWindowTitle(windowTitle, targetDesktopIndex);

            if (result.ErrorOccured)
            {
                _terminalService.WriteResultToLog(result, $"Beim Verschieben des Fensters \"{windowTitle}\" auf den virt. Desktop #{targetDesktopIndex} ist ein Fehler aufgetreten.");
            }
        }

        public bool SwitchingIsAllowed()
        {
            Alert latestAlert = _alertService.GetLatestEnabledAlert();
            if (latestAlert == null)
            {
                return true;
            }

            AlarmKonfiguration alarmKonfiguration = _configurationFileRepository.GetConfigFromJsonFile<AlarmKonfiguration>();
            Guard.IsNotNull(alarmKonfiguration, nameof(AlarmKonfiguration));

            double minutenSeitLetzterAlarmierung = Math.Round((DateTime.Now - latestAlert.AlertTimestamp).TotalMinutes, 2);
            if (minutenSeitLetzterAlarmierung < alarmKonfiguration.SperrfristInMinuten.Value)
            {
                double endeDerSperrfristInMinuten = alarmKonfiguration.SperrfristInMinuten.Value - minutenSeitLetzterAlarmierung;
                _logService.Info($"Kein Desktopwechsel erlaubt: Seit der letzten Alarmierung sind erst {minutenSeitLetzterAlarmierung} Minuten vergangen. Der nächste Wechsel ist erst in {endeDerSperrfristInMinuten} Minuten erlaubt.");
                return false;
            }

            return true;
        }

        private int GetIntFromKeyValueString(char separator, string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return default;
            }
            int indexAfterSeparator = input.LastIndexOf(separator) + 1;

            string countAsString = input.Substring(indexAfterSeparator, input.Length - indexAfterSeparator).Trim();
            if (int.TryParse(countAsString, out int countAsNumber))
            {
                return countAsNumber;
            }

            return default;
        }

    }
}
