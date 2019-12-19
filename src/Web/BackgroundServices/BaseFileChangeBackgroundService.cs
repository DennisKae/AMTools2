using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using Microsoft.Extensions.Hosting;

namespace AMTools.Web.BackgroundServices
{
    public abstract class BaseFileChangeBackgroundService : BackgroundService
    {
        private readonly ILogService _logService;

        private FileSystemWatcher _fileSystemWatcher;

        private string _targetFilepath;

        public BaseFileChangeBackgroundService(
            ILogService logService)
        {
            _logService = logService;
        }

        protected abstract override Task ExecuteAsync(CancellationToken stoppingToken);

        protected abstract void OnFileChange();

        protected abstract void OnExceptionAfterFileChange();

        /// <summary>
        /// Initialisiert den BackgroundService:
        /// <para>- Erstellt den FileSystemWatcher</para>
        /// <para>- Löst das Event einmalig aus</para>
        /// </summary>
        protected void InitializeBackgroundService(string filepath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filepath) || !File.Exists(filepath))
                {
                    throw new FileNotFoundException(filepath);
                }

                _targetFilepath = filepath;
                string targetDirectory = Path.GetDirectoryName(filepath);
                string fileName = Path.GetFileName(filepath);

                _fileSystemWatcher = new FileSystemWatcher(targetDirectory)
                {
                    NotifyFilter = NotifyFilters.LastWrite,
                    Filter = "*" + fileName
                };

                _fileSystemWatcher.Changed += new FileSystemEventHandler(InternalOnFileChange);
                _fileSystemWatcher.Created += new FileSystemEventHandler(InternalOnFileChange);
                _fileSystemWatcher.EnableRaisingEvents = true;

                _logService.Info(GetType().Name + " mit folgendem Pfad initialisiert: " + Environment.NewLine + filepath);

                ExecuteOnFileChange();
            }
            catch (Exception exception)
            {
                _logService.Exception(exception, GetType().Name + ": Bei der Initialisierung trat eine Exception auf.");
            }
        }

        // Das Event wird mehrfach ausgeführt?! Bekannter Bug:
        // https://github.com/Microsoft/dotnet/issues/347
        // https://github.com/dotnet/corefx/issues/25117

        private void InternalOnFileChange(object sender, FileSystemEventArgs eventArgs)
        {
            _logService.Info(GetType().Name + ": Dateiänderung erkannt.");
            if (!File.Exists(_targetFilepath))
            {
                _logService.Error(GetType().Name + ": Die überwachte Datei ist nicht mehr unter dem folgenden Pfad auffindbar: " + Environment.NewLine + _targetFilepath);
                return;
            }

            OnFileChange();
        }

        private void ExecuteOnFileChange()
        {
            try
            {
                OnFileChange();
            }
            catch (Exception exception)
            {
                _logService.Exception(exception, GetType().Name + ": Beim Verarbeiten der Dateiänderung trat eine Exception auf.");

                OnExceptionAfterFileChange();
            }
        }
    }
}
