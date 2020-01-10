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
    public class QualificationSettingMappingAction : IMappingAction<Setting, QualificationSettingViewModel>
    {
        public void Process(Setting source, QualificationSettingViewModel destination, ResolutionContext context)
        {
            destination.Abkuerzung = source?.Key;
            destination.Bezeichnung = source?.Value;
        }
    }
}
