using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Core.Services.Settings.Interfaces;
using AMTools.Web.Core.ViewModels;
using AMTools.Web.Data.Database.Models;
using AutoMapper;

namespace AMTools.Web.Core.Mappings
{
    public class AvailabilityStatusMappingAction : IMappingAction<DbAvailabilityStatus, AvailabilityStatusViewModel>, IMappingAction<AvailabilityStatus, AvailabilityStatusViewModel>
    {
        private readonly IAvailabilityStatusSettingService _availabilityStatusSettingService;

        public AvailabilityStatusMappingAction(
            IAvailabilityStatusSettingService availabilityStatusSettingService)
        {
            _availabilityStatusSettingService = availabilityStatusSettingService;
        }

        public void Process(DbAvailabilityStatus source, AvailabilityStatusViewModel destination, ResolutionContext context)
        {
            destination.Setting = _availabilityStatusSettingService.GetByNumber(source.Value.ToString());
        }

        public void Process(AvailabilityStatus source, AvailabilityStatusViewModel destination, ResolutionContext context)
        {
            destination.Setting = _availabilityStatusSettingService.GetByNumber(source.Value.ToString());
        }
    }
}
