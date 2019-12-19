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
    public class SubGroupService : ISubGroupService
    {
        private readonly char _subGroupNameSeparator = ',';

        private readonly IMapper _mapper;
        private readonly ISettingsService _settingsService;

        public SubGroupService(
            IMapper mapper,
            ISettingsService settingsService)
        {
            _mapper = mapper;
            _settingsService = settingsService;
        }

        public List<SubGroupViewModel> GetAll() => _mapper.Map<List<SubGroupViewModel>>(_settingsService.GetByCategoryName(SettingCategoryNames.SubGroups));

        public List<string> GetSubGroupNamesFromAlertText(string alertText)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(alertText) || !alertText.Contains('-'))
            {
                return result;
            }

            // 03.11. 20:29:33 - ID: 244, Schweregrad 6 - Musterhausen - Subgruppe(n): MUSTERHAUSEN_PAGER_VOLLALARM - S01*FUNKTIONSPROBE

            string[] splittedAlertText = alertText.Split('-');
            if (splittedAlertText.Length < 4)
            {
                return result;
            }

            string targetAlertText = splittedAlertText[3].Replace("Subgruppe(n):", "").Trim();
            List<string> subGroupNames = targetAlertText.Split(_subGroupNameSeparator).ToList();
            subGroupNames.ForEach(x => x = x.Trim());

            return subGroupNames;
        }

        public List<SubGroupViewModel> GetSubGroupsFromAlertText(string alertText)
        {
            var result = new List<SubGroupViewModel>();
            List<string> subGroupNames = GetSubGroupNamesFromAlertText(alertText);
            if (subGroupNames == null || subGroupNames.Count == 0)
            {
                return result;
            }

            List<SubGroupViewModel> allSubGroups = GetAll();
            if (allSubGroups == null || allSubGroups.Count == 0)
            {
                return result;
            }

            foreach (string subGroupName in subGroupNames)
            {
                SubGroupViewModel matchingGroup = allSubGroups.FirstOrDefault(x => x.Bezeichnung.Equals(subGroupName, StringComparison.InvariantCultureIgnoreCase));
                if (matchingGroup != null)
                {
                    result.Add(matchingGroup);
                }
            }

            return result;
        }
    }
}
