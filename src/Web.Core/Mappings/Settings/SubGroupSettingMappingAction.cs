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
    public class SubGroupSettingMappingAction : IMappingAction<Setting, SubGroupSettingViewModel>
    {
        public void Process(Setting source, SubGroupSettingViewModel destination, ResolutionContext context)
        {
            destination.Nummer = int.TryParse(source?.Key, out int parsedNumber) ? parsedNumber : default;
            destination.Bezeichnung = source?.Value;
        }
    }
}
