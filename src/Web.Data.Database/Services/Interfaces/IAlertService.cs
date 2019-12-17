using AMTools.Shared.Core.Models;

namespace AMTools.Web.Data.Database.Services.Interfaces
{
    public interface IAlertService
    {
        Alert GetLatestEnabledAlert();
    }
}