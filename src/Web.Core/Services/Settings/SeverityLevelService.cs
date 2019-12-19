using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Web.Core.Services.Settings.Interfaces;
using AMTools.Web.Core.ViewModels;
using AutoMapper;

namespace AMTools.Web.Core.Services.Settings
{
    public class SeverityLevelService : ISeverityLevelService
    {
        private readonly IMapper _mapper;
        private readonly ISettingsService _settingsService;

        public SeverityLevelService(
            IMapper mapper,
            ISettingsService settingsService)
        {
            _mapper = mapper;
            _settingsService = settingsService;
        }

        public List<SeverityLevelViewModel> GetAll() => _mapper.Map<List<SeverityLevelViewModel>>(_settingsService.GetByCategoryName(SettingCategoryNames.SeverityLevels));

        public string GetSeverityLevelTextFromAlertText(string alertText)
        {
            if (string.IsNullOrWhiteSpace(alertText) || !alertText.Contains('-'))
            {
                return null;
            }

            // 03.11. 20:29:33 - ID: 244, Schweregrad 6 - Musterhausen - Subgruppe(n): MUSTERHAUSEN_PAGER_VOLLALARM - S01*FUNKTIONSPROBE

            string[] splittedAlertText = alertText.Split('-');
            if (splittedAlertText.Length < 2)
            {
                return null;
            }

            string[] targetAlertTextSplitted = splittedAlertText[1].Split(',');
            if (targetAlertTextSplitted.Length >= 2)
            {
                return targetAlertTextSplitted[1].Trim();
            }

            return null;
        }

        public SeverityLevelViewModel GetSeverityLevelFromAlertText(string alertText)
        {
            string severityLevelText = GetSeverityLevelTextFromAlertText(alertText);
            if (string.IsNullOrWhiteSpace(severityLevelText))
            {
                return null;
            }
            List<SeverityLevelViewModel> allSeverityLevels = GetAll();
            if (allSeverityLevels == null || allSeverityLevels.Count == 0)
            {
                return null;
            }

            SeverityLevelViewModel matchingSeverityLevel = allSeverityLevels.FirstOrDefault(x => x.Bezeichnung.Equals(severityLevelText, StringComparison.InvariantCultureIgnoreCase));
            if (matchingSeverityLevel != null)
            {
                return matchingSeverityLevel;
            }

            return null;
        }
    }
}
