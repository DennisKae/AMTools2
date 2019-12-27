using AMTools.Web.Core.ViewModels;

namespace AMTools.Web.Core.Services.Interfaces
{
    public interface IAvailabilityStatusService
    {
        AvailabilityStatusViewModel GetByIssi(string issi);
    }
}