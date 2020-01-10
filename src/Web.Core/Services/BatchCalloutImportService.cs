using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core;
using AMTools.Shared.Core.Models.Konfigurationen;
using AMTools.Shared.Core.Repositories.Interfaces;
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Data.Files.Repositories.Interfaces;

namespace AMTools.Web.Core.Services
{
    public class BatchCalloutImportService : IBatchCalloutImportService
    {
        private readonly IConfigurationFileRepository _configurationFileRepository;
        private readonly ILogService _logService;

        public BatchCalloutImportService(
            IConfigurationFileRepository configurationFileRepository,
            ILogService logService)
        {
            _configurationFileRepository = configurationFileRepository;
            _logService = logService;
        }

        /// <summary>Importiert die Callout-Dateien aus dem angegebenen Ordner</summary>
        public void Import(string inputDirectory, string inputFilenamePattern)
        {
            Guard.IsNotNull(inputDirectory, nameof(inputDirectory));
            Guard.IsNotNull(inputFilenamePattern, nameof(inputFilenamePattern));
            if (!Directory.Exists(inputDirectory))
            {
                _logService.Error($"Das angegebene {nameof(inputDirectory)} existiert nicht: " + Environment.NewLine + inputDirectory);
                return;
            }

            List<string> sourceFiles = Directory.GetFiles(inputDirectory, inputFilenamePattern, SearchOption.AllDirectories).OrderBy(x => x).ToList();
            if (sourceFiles == null || sourceFiles.Count == 0)
            {
                _logService.Error("Im angegebenen Ordner konnten keine Dateien zur Verarbeitung gefunden werden.");
                return;
            }

            DateiKonfiguration dateiKonfiguration = _configurationFileRepository.GetConfigFromJsonFile<DateiKonfiguration>();
            string targetFile = dateiKonfiguration?.CalloutDatei;
            if (string.IsNullOrWhiteSpace(targetFile) || !File.Exists(targetFile))
            {
                _logService.Error("Unter folgendem Pfad konnte keine Callout-Datei gefunden werden: " + Environment.NewLine + targetFile);
                return;
            }

            int emptyFileCounter = 0;
            int processedFileCounter = 0;
            foreach (string sourceFile in sourceFiles)
            {
                string fileContents = File.ReadAllText(sourceFile);
                if (string.IsNullOrWhiteSpace(fileContents) || fileContents.Replace(Environment.NewLine, "") == "<?xml version=\"1.0\" encoding=\"utf-8\"?><Alerts />")
                {
                    emptyFileCounter++;
                    _logService.Info("Diese Datei ist leer (oder enthält keine alerts) und wird nicht verarbeitet: " + Environment.NewLine + sourceFile);
                    continue;
                }

                File.WriteAllText(targetFile, fileContents);
                processedFileCounter++;
                Thread.Sleep(2000);
            }

            _logService.Info($"Verarbeitung abgeschlossen:" + Environment.NewLine + $"- {processedFileCounter} Datei(en) wurden verarbeitet" + Environment.NewLine + $"- {emptyFileCounter} Datei(en) waren leer oder enthielten keine alerts");
        }
    }
}
