using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Web.Core.ViewModels.Settings;
using AutoMapper;

namespace AMTools.Web.Core.Mappings.Settings
{
    public class AvailabilityStatusSettingMappingAction : IMappingAction<Setting, AvailabilityStatusSettingViewModel>
    {
        public void Process(Setting source, AvailabilityStatusSettingViewModel destination, ResolutionContext context)
        {
            destination.Nummer = source.Key;
            destination.Bezeichnung = source.Value;
            destination.Farbe = source.Color;
        }
    }
}
