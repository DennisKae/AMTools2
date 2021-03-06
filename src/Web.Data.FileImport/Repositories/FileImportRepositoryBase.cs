﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Models;

namespace AMTools.Web.Data.Files.Repositories
{
    public class FileImportRepositoryBase
    {
        private readonly ILogService _logService;

        public FileImportRepositoryBase(ILogService logService)
        {
            _logService = logService;
        }

        protected bool FileExistsAndIsNotEmpty(string propertyName, string filepath)
        {
            if (string.IsNullOrWhiteSpace(filepath) || !File.Exists(filepath))
            {
                _logService.Error($"Konfigurationsproblem: {propertyName} nicht gefunden.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(File.ReadAllText(filepath)))
            {
                _logService.Info("Die folgende Datei ist leer: " + filepath);
                return false;
            }

            return true;
        }

        protected string GetTimestampFromDateTime(DateTime dateTime) => dateTime.ToString("yyyy-MM-dd,HH:mm:ss", CultureInfo.InvariantCulture);

        /// <summary>
        /// Parsed ein DateTime aus einem string im Format "03.11. 20:29:42"
        /// </summary>
        /// <param name="alertTimestamp">Zu parsender string</param>
        /// <param name="previousDateTime">Datum, das vor dem zu parsenden Datum liegt.</param>
        /// <returns></returns>
        protected DateTime GetDateTimeFromAlertTimestamp(string alertTimestamp, DateTime previousDateTime)
        {
            if (string.IsNullOrWhiteSpace(alertTimestamp))
            {
                return new DateTime();
            }

            int resultMonth = int.TryParse(alertTimestamp.Split('.')[1], out int parsedMonth) ? parsedMonth : previousDateTime.Month;

            // Ist der Monat des zu parsenden Datums kleiner als der Monat des vorherigen Datums, dann liegt das zu parsende Datum im nächsten Jahr.
            int year = resultMonth < previousDateTime.Month ? previousDateTime.Year + 1 : previousDateTime.Year;

            alertTimestamp = alertTimestamp.Trim().Replace(" ", year + ",");
            var result = DateTime.TryParse(alertTimestamp, out DateTime parsedTimestamp) ? parsedTimestamp : default;

            return result;
        }
    }
}
