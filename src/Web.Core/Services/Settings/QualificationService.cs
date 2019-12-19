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
    public class QualificationService : IQualificationService
    {
        private readonly char _qualificationStringSeparator = ',';
        private readonly IMapper _mapper;
        private readonly ISettingsService _settingsService;

        public QualificationService(
            IMapper mapper,
            ISettingsService settingsService)
        {
            _mapper = mapper;
            _settingsService = settingsService;
        }

        public List<QualificationViewModel> GetAll()
        {
            return _mapper.Map<List<QualificationViewModel>>(_settingsService.GetByCategoryName(SettingCategoryNames.Qualifications));
        }

        /// <summary>Liefert die Qualifikationen eines strings im Format "AGT,DEFAULT"</summary>
        public List<QualificationViewModel> GetByReferenceString(string referenceString)
        {
            var result = new List<QualificationViewModel>();
            if (string.IsNullOrWhiteSpace(referenceString))
            {
                return result;
            }

            List<QualificationViewModel> allPossibleQualifications = GetAll();
            if (allPossibleQualifications == null || allPossibleQualifications.Count == 0)
            {
                return result;
            }

            List<string> referenceStringSplitted = referenceString.Split(new[] { _qualificationStringSeparator }, StringSplitOptions.RemoveEmptyEntries).ToList();
            referenceStringSplitted.ForEach(x => x = x.Trim().ToLowerInvariant());

            return allPossibleQualifications.Where(x => referenceStringSplitted.Any(y => x.Abkuerzung.Trim().Equals(y, StringComparison.InvariantCultureIgnoreCase))).ToList();
        }
    }
}
