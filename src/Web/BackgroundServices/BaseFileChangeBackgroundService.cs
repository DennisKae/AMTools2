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

        public BaseFileChangeBackgroundService(
            ILogService logService)
        {
            _logService = logService;
        }

        protected abstract override Task ExecuteAsync(CancellationToken stoppingToken);

        protected abstract void OnFileChange(object sender, FileSystemEventArgs eventArgs);

        protected void InitializeFileSystemWatcher(string filepath)
        {
            if (string.IsNullOrWhiteSpace(filepath) || !File.Exists(filepath))
            {
                throw new FileNotFoundException(filepath);
            }

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
        }

        private void InternalOnFileChange(object sender, FileSystemEventArgs eventArgs)
        {
            _logService.Info(GetType().Name + ": Dateiänderung erkannt.");

            OnFileChange(sender, eventArgs);
        }
    }
}
