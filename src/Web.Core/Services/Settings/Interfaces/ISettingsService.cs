using System.Collections.Generic;
using AMTools.Shared.Core.Models;

namespace AMTools.Web.Core.Services.Settings.Interfaces
{
    public interface ISettingsService
    {
        void ClearMemoryCache();
        List<Setting> GetAll();
        List<string> GetAllSettingNames();
        List<Setting> GetByCategoryName(string categoryName);
    }
}