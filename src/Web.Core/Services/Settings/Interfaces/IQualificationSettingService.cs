using System.Collections.Generic;
using AMTools.Web.Core.ViewModels.Settings;

namespace AMTools.Web.Core.Services.Settings.Interfaces
{
    public interface IQualificationSettingService
    {
        List<QualificationSettingViewModel> GetAll();
        List<QualificationSettingViewModel> GetByReferenceString(string referenceString);
    }
}