using System.Collections.Generic;
using AMTools.Shared.Core.Models;

namespace AMTools.Web.Data.Files.Repositories.Interfaces
{
    public interface ISettingsFileRepository
    {
        List<string> GetAllSettingNames();
        List<Setting> GetAllSettings();
        List<Setting> GetSetting(string settingName);
    }
}