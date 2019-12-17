using System.Collections.Generic;
using AMTools.Shared.Core.Models;

namespace AMTools.Web.Core.Services.Interfaces
{
    public interface ISettingsService
    {
        List<Setting> GetAll();
        List<Setting> GetByCategoryName(string categoryName);
    }
}